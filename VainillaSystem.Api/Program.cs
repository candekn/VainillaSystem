using VainillaSystem.Application.Features.UserCQRS.Commands;
using VainillaSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VainillaSystem API", Version = "v1" });
});

// Register Infrastructure: MyVanillaMediator + Behaviors + InMemoryUserRepository.
// Pass the Application assembly so the DI scanner finds all handlers.
builder.Services.AddInfrastructure(typeof(CreateUserCommand).Assembly);

// ---------- Pipeline ----------
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();