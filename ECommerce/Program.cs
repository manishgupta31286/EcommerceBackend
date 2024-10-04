using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddResponseCaching();

builder.Services.AddDbContext<EcommerceContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("MyDatabaseConnection")));

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
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseCors("AllowOrigin");
app.UseHttpsRedirection();

app.UseRouting();

app.UseResponseCaching();
app.MapControllers();

app.Run();
