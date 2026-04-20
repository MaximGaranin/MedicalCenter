using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MedicalCenter.Application.Appointments;
using MedicalCenter.Application.Doctors;
using MedicalCenter.Application.Doctors.Abstractions;
using MedicalCenter.Application.Patients;
using MedicalCenter.Application.Patients.Abstractions;
using MedicalCenter.Application.Appointments.Abstractions;
using MedicalCenter.Infrastructure.Doctors;
using MedicalCenter.Infrastructure.Patients;
using MedicalCenter.Infrastructure.Appointments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDoctorsRepository, DoctorsRepository>();
builder.Services.AddSingleton<IPatientsRepository, PatientsRepository>();
builder.Services.AddSingleton<IAppointmentsRepository, AppointmentsRepository>();

builder.Services.AddScoped<DoctorsService>();
builder.Services.AddScoped<PatientsService>();
builder.Services.AddScoped<AppointmentsService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Medical Center API",
        Version = "v1",
        Description = "База данных медицинского центра: врачи, пациенты, расписание"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Center API V1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();
app.MapControllers();
app.Run();
