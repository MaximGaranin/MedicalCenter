using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MedicalCenter.Application.Appointments;
using MedicalCenter.Application.Doctors.Abstractions;
using MedicalCenter.Application.Patients.Abstractions;
using MedicalCenter.Domain.Appointments;
using MedicalCenter.Presentation.Contracts;

namespace MedicalCenter.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentsService _service;
        private readonly IDoctorsRepository _doctorsRepo;
        private readonly IPatientsRepository _patientsRepo;

        public AppointmentsController(
            AppointmentsService service,
            IDoctorsRepository doctorsRepo,
            IPatientsRepository patientsRepo)
        {
            _service = service;
            _doctorsRepo = doctorsRepo;
            _patientsRepo = patientsRepo;
        }

        /// <summary>Записать пациента к врачу</summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreateAppointmentRequest request)
        {
            try
            {
                var id = _service.CreateAppointment(
                    request.DoctorId, request.PatientId, request.AppointmentTime);
                return Ok(new { id });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Удалить запись</summary>
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            try { _service.DeleteAppointment(id); return NoContent(); }
            catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>Расписание врача на неделю (передать дату понедельника)</summary>
        [HttpGet("doctor/{doctorId:guid}/week")]
        public IActionResult GetDoctorWeek(Guid doctorId, [FromQuery] DateTime weekStart)
            => Ok(_service.GetWeekScheduleForDoctor(doctorId, weekStart).Select(MapToResponse));

        /// <summary>Записи врача на сегодня</summary>
        [HttpGet("doctor/{doctorId:guid}/today")]
        public IActionResult GetDoctorToday(Guid doctorId)
            => Ok(_service.GetTodayAppointmentsForDoctor(doctorId).Select(MapToResponse));

        /// <summary>Записи пациента на сегодня</summary>
        [HttpGet("patient/{patientId:guid}/today")]
        public IActionResult GetPatientToday(Guid patientId)
            => Ok(_service.GetTodayAppointmentsForPatient(patientId).Select(MapToResponse));

        /// <summary>Все записи врача</summary>
        [HttpGet("doctor/{doctorId:guid}")]
        public IActionResult GetAllByDoctor(Guid doctorId)
            => Ok(_service.GetAllByDoctor(doctorId).Select(MapToResponse));

        /// <summary>Все записи пациента</summary>
        [HttpGet("patient/{patientId:guid}")]
        public IActionResult GetAllByPatient(Guid patientId)
            => Ok(_service.GetAllByPatient(patientId).Select(MapToResponse));

        private AppointmentResponse MapToResponse(Appointment a)
        {
            var doctor = _doctorsRepo.GetById(a.DoctorId);
            var patient = _patientsRepo.GetById(a.PatientId);
            return new AppointmentResponse(
                a.Id,
                a.DoctorId,
                doctor != null ? $"{doctor.LastName} {doctor.FirstName} {doctor.Patronymic}" : "—",
                a.PatientId,
                patient != null ? $"{patient.LastName} {patient.FirstName} {patient.Patronymic}" : "—",
                a.AppointmentTime,
                a.EndTime);
        }
    }
}
