using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MedicalCenter.Application.Doctors;
using MedicalCenter.Domain.Doctors;
using MedicalCenter.Presentation.Contracts;

namespace MedicalCenter.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorsService _service;

        public DoctorsController(DoctorsService service)
        {
            _service = service;
        }

        /// <summary>Получить всех врачей</summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var doctors = _service.GetAllDoctors().Select(MapToResponse);
            return Ok(doctors);
        }

        /// <summary>Получить врача по Id</summary>
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var doctor = _service.GetDoctor(id);
                return Ok(MapToResponse(doctor));
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Поиск врачей по фамилии</summary>
        [HttpGet("search")]
        public IActionResult SearchByLastName([FromQuery] string lastName)
        {
            var doctors = _service.SearchByLastName(lastName).Select(MapToResponse);
            return Ok(doctors);
        }

        /// <summary>Добавить врача</summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreateDoctorRequest request)
        {
            try
            {
                var id = _service.CreateDoctor(
                    request.FirstName,
                    request.LastName,
                    request.Patronymic,
                    request.Specialization,
                    request.Phone,
                    request.Shift);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Удалить врача</summary>
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _service.DeleteDoctor(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private static DoctorResponse MapToResponse(Doctor d) => new(
            d.Id,
            d.FirstName,
            d.LastName,
            d.Patronymic,
            d.Specialization,
            d.Phone,
            d.Shift == Shift.Morning ? "Утренняя" : "Вечерняя",
            d.Shift == Shift.Morning ? "08:00–14:00" : "14:00–20:00"
        );
    }
}
