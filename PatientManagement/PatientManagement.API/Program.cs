using FluentValidation;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.Services;
using PatientManagement.API.Middleware;
using PatientManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<PatientManagement.Application.Mapping.PatientProfile>();
});

builder.Services.AddValidatorsFromAssemblyContaining<PatientManagement.Application.Mapping.PatientProfile>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
