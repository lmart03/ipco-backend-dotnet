using System.Diagnostics;
using Ipco.Api.Configuration;
using Ipco.Infrastructure;
using Ipco.Modules.Patients;
using Ipco.Modules.Patients.Presentation;
using Ipco.Web.Problems;
using Scalar.AspNetCore;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ProblemDetailsDefaults.Customize);
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddIpcoInfrastructure();
builder.Services.AddPatientsModule(builder.Configuration);

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.MapHealthChecks("/health/live");
app.MapPatientsModule();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("IPCO API"));
}

app.Run();
public partial class Program;
