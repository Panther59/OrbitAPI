
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using Orbit.Core.Extensions;
using Orbit.Models;
using Orbit.Models.Settings;
using OrbitAPI.Middlewares;
using System.Text;
using System.Text.Json;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
	logger.Info("Iniatializing Orbit API");

	var builder = WebApplication.CreateBuilder(args);
	ConfigurationManager configuration = builder.Configuration;

	builder.Logging.ClearProviders();
	builder.Host.UseNLog();
	// Add services to the container.

	builder.Services.AddOrbitCore();
	builder.Services.AddControllers().AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	});

	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("Jwt"));
	builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
	builder.Services.Configure<GoogleSetting>(builder.Configuration.GetSection("Google"));

	builder.Services.AddAuthentication(x =>
	{
		x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	}).AddCookie(x =>
	{
		x.Cookie.Name = "token";

	}).AddJwtBearer(x =>
	{
		x.RequireHttpsMetadata = false;
		x.SaveToken = true;
		x.TokenValidationParameters = new TokenValidationParameters
		{
			ValidIssuer = configuration["Jwt:Issuer"],
			ValidAudience = configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = false,
			ValidateIssuerSigningKey = true
		};
		x.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				context.Token = context.Request.Cookies["X-Access-Token"];
				return Task.CompletedTask;
			}
		};

	});

	//builder.Services.AddAuthentication(options =>
	//{
	//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	//	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	//}).AddJwtBearer(o =>
	//{
	//	o.TokenValidationParameters = new TokenValidationParameters
	//	{
	//		ValidIssuer = configuration["Jwt:Issuer"],
	//		ValidAudience = configuration["Jwt:Audience"],
	//		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
	//		ValidateIssuer = true,
	//		ValidateAudience = true,
	//		ValidateLifetime = false,
	//		ValidateIssuerSigningKey = true
	//	};
	//});

	builder.Services.AddAuthorization(options =>
	{
		options.AddPolicy(AuthorizationPolicies.Admin.ToString(), policy => policy.RequireRole("Admin"));
		options.AddPolicy(AuthorizationPolicies.Admin.ToString(), policy => policy.RequireRole("ITAdmin"));
	});
	builder.Services.AddHttpClient();

	builder.Services.AddCors(opt =>
	{
		opt.AddPolicy(name: "CorsPolicy", builder =>
		{
			builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials();
		});
	});

	var app = builder.Build();

	app.UseGlobalErrorHandlingMiddleware();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseCors("CorsPolicy");

	app.UseHttpsRedirection();

	app.UseAuthentication();
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
