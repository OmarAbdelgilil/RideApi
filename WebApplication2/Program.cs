using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApplication1.Data;
using WebApplication2.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultDbContext"));
});
builder.Services.AddScoped<IDataRepository<Passanger>, DataRepository<Passanger>>();
builder.Services.AddScoped<IDataRepository<Driver>, DataRepository<Driver>>();
builder.Services.AddScoped<IDataRepository<Credentials>, DataRepository<Credentials>>();
builder.Services.AddScoped<IDataRepository<Rides>, DataRepository<Rides>>();
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
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
