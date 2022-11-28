using backend.Models;
using backend.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GestaoDbContext>();
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
builder.Services.AddScoped<IFeriasRepository, FeriasRepository>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers()
    .AddJsonOptions(x => {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .AddFluentValidation(x => {
        x.DisableDataAnnotationsValidation = true;
        x.RegisterValidatorsFromAssemblyContaining(typeof(Program));
    });

builder.Services
    .AddProblemDetails(x => {
        x.OnBeforeWriteDetails = (ctx, problem) => {
            var actionDescription = ctx.GetEndpoint()?.Metadata.GetMetadata<DescriptionAttribute>()?.Description;
            if (!string.IsNullOrEmpty(actionDescription))
                problem.Title = $"Erro ao {actionDescription}";
        };
    })
    .AddProblemDetailsConventions();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => {
    x.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseProblemDetails();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
