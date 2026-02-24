namespace VainillaSystem.Domain.Interfaces
{
    /// <summary>
    /// Marker interface that identifies a request object and associates it with its response type.
    /// All commands and queries must implement this interface.
    /// </summary>
    /// <typeparam name="TResponse">The type of the value returned by the handler.</typeparam>
    public interface IRequest<out TResponse>
    {
    }
}