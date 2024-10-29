using ECommerce;
using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Google.Cloud.Logging.V2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Formatting.Json;
using Google.Api;
using Serilog.Sinks.GoogleCloudLogging;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

string credentialsPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "seedangularecommerce-49e6c5bfaeff.json");
GoogleCredential credential;
using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
{
	credential = GoogleCredential.FromStream(stream);
}
var channel = credential.ToChannelCredentials();

var loggingClient = new LoggingServiceV2ClientBuilder
{
	ChannelCredentials = credential.ToChannelCredentials()
}.Build();

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug() // Set minimum log level
	.WriteTo.GoogleCloudLogging("seedangularecommerce", "seedangularecommerce") // Project ID and Log Name
	.WriteTo.Console()
	.CreateLogger();

using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
{
	credential = GoogleCredential.FromStream(stream);
}

var googleCredentialJson = System.IO.File.ReadAllText(credentialsPath);

//Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.GoogleCloudLogging(new GoogleCloudLoggingSinkOptions()
	{
		ProjectId = "seedangularecommerce",
		ResourceType = "global",
		GoogleCredentialJson = googleCredentialJson
	})
	.CreateLogger();


builder.Host.UseSerilog();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<EcommerceContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("MyDatabaseConnection")));

builder.Services.AddSingleton<CacheManager>();


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.Authority = builder.Configuration["Jwt:Issuer"]; // https://accounts.google.com
	options.Audience = builder.Configuration["Jwt:Audience"]; // Your Google Client ID	
	options.Events = new JwtBearerEvents
	{
		OnAuthenticationFailed = context =>
		{
			return Task.CompletedTask;
		},
		OnTokenValidated = context =>
		{
			return Task.CompletedTask;
		}
	};
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowOrigin",
		builder =>
		{
			builder.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceContext>();
	dbContext.Database.EnsureCreated(); // Creates the database if it doesn't exist
}
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");


app.UseCors("AllowOrigin");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();
app.MapControllers();

app.Run();
