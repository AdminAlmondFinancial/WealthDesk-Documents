var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app);
app.Run();

// Lets WebApplicationFactory<Program> find the entry point from the test projects.
public partial class Program;
