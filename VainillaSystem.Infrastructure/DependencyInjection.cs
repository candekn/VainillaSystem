using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using VainillaSystem.Application.Interfaces;
using VainillaSystem.Domain.Interfaces;
using VainillaSystem.Infrastructure.Behaviors;
using VainillaSystem.Infrastructure.Mediator;
using VainillaSystem.Infrastructure.Persistence;

namespace VainillaSystem.Infrastructure
{
    /// <summary>
    /// Extension methods for wiring up Infrastructure services into <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all Infrastructure services required by the application:
        /// <list type="number">
        ///   <item>Scans <paramref name="applicationAssembly"/> for all concrete <see cref="IRequestHandler{TRequest,TResponse}"/> implementations.</item>
        ///   <item>Registers each concrete handler as <c>Transient</c> (innermost layer).</item>
        ///   <item>Wraps each handler with the decorator chain: Concrete → CachingBehavior → InvalidateCacheBehavior → LoggingBehavior (outermost, runs first).</item>
        ///   <item>Registers <see cref="MyVanillaMediator"/>, <see cref="InMemoryUserRepository"/> and <see cref="IMemoryCache"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="services">The DI service collection to configure.</param>
        /// <param name="applicationAssembly">
        /// The assembly to scan for handler implementations, typically the Application project assembly.
        /// </param>
        /// <returns>The configured <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            Assembly applicationAssembly)
        {
            services.AddMemoryCache();
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();

            // Discover all concrete IRequestHandler<TRequest,TResponse> in the application assembly
            var handlerInterfaceType = typeof(IRequestHandler<,>);

            var handlerTypes = applicationAssembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
                .Where(x => x.iface.IsGenericType &&
                             x.iface.GetGenericTypeDefinition() == handlerInterfaceType)
                .ToList();

            foreach (var entry in handlerTypes)
            {
                var serviceType   = entry.iface;                              // IRequestHandler<TReq, TRes>
                var concreteType  = entry.type;                               // e.g. CreateUserHandler
                var typeArgs      = serviceType.GetGenericArguments();        // [TReq, TRes]
                var tRequest      = typeArgs[0];
                var tResponse     = typeArgs[1];

                // 1. Register the concrete handler as transient (inner-most)
                services.AddTransient(concreteType);

                // 2. Build decorator chain via factory lambdas.
                //    The last registration wins for IRequestHandler<TReq,TRes>.
                //    Order: Concrete → Caching/Invalidate → Logging (outermost, runs first).

                // --- Caching decorator (for ICachableRequest) ---
                var cachingBehaviorType = typeof(CachingBehavior<,>).MakeGenericType(tRequest, tResponse);
                services.AddTransient(serviceType, sp =>
                {
                    var inner  = (dynamic)sp.GetRequiredService(concreteType);
                    var cache  = sp.GetRequiredService<IMemoryCache>();
                    var logger = GetLogger(sp, cachingBehaviorType);
                    return ActivatorUtilities.CreateInstance(sp, cachingBehaviorType, inner, cache, logger);
                });

                // --- InvalidateCache decorator (for IInvalidateCacheRequest) ---
                var invalidateBehaviorType = typeof(InvalidateCacheBehavior<,>).MakeGenericType(tRequest, tResponse);
                services.AddTransient(serviceType, sp =>
                {
                    // resolve the previously registered service (CachingBehavior wrapping Concrete)
                    var inner  = ResolveLastHandler(sp, serviceType);
                    var cache  = sp.GetRequiredService<IMemoryCache>();
                    var logger = GetLogger(sp, invalidateBehaviorType);
                    return ActivatorUtilities.CreateInstance(sp, invalidateBehaviorType, inner, cache, logger);
                });

                // --- Logging decorator (outermost) ---
                var loggingBehaviorType = typeof(LoggingBehavior<,>).MakeGenericType(tRequest, tResponse);
                services.AddTransient(serviceType, sp =>
                {
                    var inner  = ResolveLastHandler(sp, serviceType);
                    var logger = GetLogger(sp, loggingBehaviorType);
                    return ActivatorUtilities.CreateInstance(sp, loggingBehaviorType, inner, logger);
                });
            }

            // Register the mediator (resolves the outermost IRequestHandler<,> from DI)
            services.AddTransient<IMediator, MyVanillaMediator>();

            return services;
        }

        /// <summary>
        /// Resolves all registrations for the given <paramref name="serviceType"/> and returns the last one,
        /// which corresponds to the most recently added decorator (outermost at the time of resolution).
        /// </summary>
        /// <param name="sp">The service provider to resolve from.</param>
        /// <param name="serviceType">The handler service type to resolve.</param>
        /// <returns>The last registered instance for <paramref name="serviceType"/>.</returns>
        private static object ResolveLastHandler(IServiceProvider sp, Type serviceType)
        {
            var all = (IEnumerable<object>)sp.GetServices(serviceType);
            return all.Last();
        }

        /// <summary>
        /// Resolves a generic <c>ILogger&lt;T&gt;</c> for the given <paramref name="behaviorType"/> from DI.
        /// </summary>
        /// <param name="sp">The service provider to resolve from.</param>
        /// <param name="behaviorType">The closed generic behavior type to use as the logger category.</param>
        /// <returns>The resolved logger instance.</returns>
        private static object GetLogger(IServiceProvider sp, Type behaviorType)
        {
            var loggerType = typeof(Microsoft.Extensions.Logging.ILogger<>).MakeGenericType(behaviorType);
            return sp.GetRequiredService(loggerType);
        }
    }
}
