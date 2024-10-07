using ECommerce;
using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

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

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowOrigin",
		builder =>
		{
			builder.WithOrigins("http://localhost:4200")
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

app.UseResponseCaching();
app.MapControllers();

app.Run();
