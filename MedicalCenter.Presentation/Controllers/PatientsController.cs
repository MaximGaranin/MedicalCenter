using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MedicalCenter.Application.Patients;
using MedicalCenter.Domain.Patients;
using MedicalCenter.Presentation.Contracts;

namespace MedicalCenter.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientsService _service;

        public PatientsController(PatientsService service)
        {
            _service = service;
        }

        /// <summary>Получить всех пациентов</summary>
        [HttpGet]
        public IActionResult GetAll()
            => Ok(_service.GetAllPatients().Select(MapToResponse));

        /// <summary>Получить пациента по Id</summary>
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            try { return Ok(MapToResponse(_service.GetPatient(id))); }
            catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>Поиск пациентов по фамилии</summary>
        [HttpGet("search")]
        public IActionResult SearchByLastName([FromQuery] string lastName)
            => Ok(_service.SearchByLastName(lastName).Select(MapToResponse));

        /// <summary>Добавить пациента</summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreatePatientRequest request)
        {
            try
            {
                var id = _service.CreatePatient(
                    request.FirstName, request.LastName, request.Patronymic,
                    request.BirthDate, request.Phone, request.Address);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Удалить пациента</summary>
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            try { _service.DeletePatient(id); return NoContent(); }
            catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        }

        private static PatientResponse MapToResponse(Patient p) => new(
            p.Id, p.FirstName, p.LastName, p.Patronymic,
            p.BirthDate, p.Phone, p.Address);
    }
}
