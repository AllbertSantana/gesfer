using backend.Models;
using backend.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GestaoDbContext>();
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(c => c.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddFluentValidation(c => {
        c.DisableDataAnnotationsValidation = true;
        c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => {
    x.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/api/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
