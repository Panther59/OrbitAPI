
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
	logger.Info("Iniatializing Orbit API");

	var builder = WebApplication.CreateBuilder(args);
	builder.Logging.ClearProviders();
	builder.Host.UseNLog();
	// Add services to the container.

	builder.Services.AddControllers();
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();

	app.Run();
}
catch (Exception exception)
{
	//NLog: catch setup errors
	logger.Error(exception, "Orbit API failed with exception");
	throw;
}
finally
{
	// Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
	NLog.LogManager.Shutdown();
}
