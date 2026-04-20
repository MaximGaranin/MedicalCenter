using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using MedicalCenter.Application.Appointments;
using MedicalCenter.Application.Appointments.Abstractions;
using MedicalCenter.Application.Doctors.Abstractions;
using MedicalCenter.Application.Patients.Abstractions;
using MedicalCenter.Domain.Appointments;
using MedicalCenter.Domain.Doctors;
using MedicalCenter.Domain.Patients;

namespace MedicalCenter.Tests.Application
{
    [TestFixture]
    public class AppointmentsServiceTests
    {
        private Mock<IAppointmentsRepository> _appointmentsRepoMock;
        private Mock<IDoctorsRepository>      _doctorsRepoMock;
        private Mock<IPatientsRepository>     _patientsRepoMock;
        private AppointmentsService           _service;

        private Doctor  _doctor;
        private Patient _patient;

        // Ближайший понедельник 10:00 (утренняя смена, кратно 30 мин)
        private static DateTime ValidMorningTime =>
            NextMonday().Date.AddHours(10);

        private static DateTime NextMonday()
        {
            var d = DateTime.Today;
            while (d.DayOfWeek != DayOfWeek.Monday) d = d.AddDays(1);
            return d;
        }

        [SetUp]
        public void SetUp()
        {
            _appointmentsRepoMock = new Mock<IAppointmentsRepository>();
            _doctorsRepoMock      = new Mock<IDoctorsRepository>();
            _patientsRepoMock     = new Mock<IPatientsRepository>();

            _service = new AppointmentsService(
                _appointmentsRepoMock.Object,
                _doctorsRepoMock.Object,
                _patientsRepoMock.Object);

            _doctor  = Doctor.Create("Пётр", "Смирнов", "", "Терапевт", "", Shift.Morning);
            _patient = Patient.Create("Иван", "Иванов", "", new DateTime(1990, 1, 1), "", "");

            _doctorsRepoMock.Setup(r => r.GetById(_doctor.Id)).Returns(_doctor);
            _patientsRepoMock.Setup(r => r.GetById(_patient.Id)).Returns(_patient);

            // По умолчанию — нет существующих записей
            _appointmentsRepoMock
                .Setup(r => r.GetByDoctorId(_doctor.Id))
                .Returns(new List<Appointment>());
            _appointmentsRepoMock
                .Setup(r => r.GetByPatientId(_patient.Id))
                .Returns(new List<Appointment>());
        }

        // ─── CreateAppointment: позитивные ───────────────────────────────

        [Test]
        public void CreateAppointment_ValidData_CallsAddOnce()
        {
            _service.CreateAppointment(_doctor.Id, _patient.Id, ValidMorningTime);

            _appointmentsRepoMock.Verify(r => r.Add(It.IsAny<Appointment>()), Times.Once);
        }

        [Test]
        public void CreateAppointment_ValidData_ReturnsNonEmptyGuid()
        {
            var id = _service.CreateAppointment(_doctor.Id, _patient.Id, ValidMorningTime);

            Assert.That(id, Is.Not.EqualTo(Guid.Empty));
        }

        // ─── CreateAppointment: врач/пациент не найдены ──────────────────

        [Test]
        public void CreateAppointment_DoctorNotFound_ThrowsException()
        {
            var unknownDoctorId = Guid.NewGuid();
            _doctorsRepoMock.Setup(r => r.GetById(unknownDoctorId)).Returns((Doctor)null);

            var ex = Assert.Throws<Exception>(() =>
                _service.CreateAppointment(unknownDoctorId, _patient.Id, ValidMorningTime));

            Assert.That(ex.Message, Does.Contain(unknownDoctorId.ToString()));
        }

        [Test]
        public void CreateAppointment_PatientNotFound_ThrowsException()
        {
            var unknownPatientId = Guid.NewGuid();
            _patientsRepoMock.Setup(r => r.GetById(unknownPatientId)).Returns((Patient)null);

            var ex = Assert.Throws<Exception>(() =>
                _service.CreateAppointment(_doctor.Id, unknownPatientId, ValidMorningTime));

            Assert.That(ex.Message, Does.Contain(unknownPatientId.ToString()));
        }

        // ─── CreateAppointment: конфликты ────────────────────────────────

        [Test]
        public void CreateAppointment_DoctorAlreadyBusy_ThrowsException()
        {
            var existingAppointment = Appointment.Create(
                _doctor.Id, _patient.Id, ValidMorningTime, Shift.Morning);

            _appointmentsRepoMock
                .Setup(r => r.GetByDoctorId(_doctor.Id))
                .Returns(new List<Appointment> { existingAppointment });

            var ex = Assert.Throws<Exception>(() =>
                _service.CreateAppointment(_doctor.Id, _patient.Id, ValidMorningTime));

            Assert.That(ex.Message, Does.Contain("уже занят"));
        }

        [Test]
        public void CreateAppointment_PatientAlreadyBooked_ThrowsException()
        {
            var anotherDoctorId = Guid.NewGuid();
            var existingAppointment = Appointment.Create(
                anotherDoctorId, _patient.Id, ValidMorningTime, Shift.Morning);

            _appointmentsRepoMock
                .Setup(r => r.GetByPatientId(_patient.Id))
                .Returns(new List<Appointment> { existingAppointment });

            var ex = Assert.Throws<Exception>(() =>
                _service.CreateAppointment(_doctor.Id, _patient.Id, ValidMorningTime));

            Assert.That(ex.Message, Does.Contain("уже записан"));
        }

        // ─── DeleteAppointment ───────────────────────────────────────────

        [Test]
        public void DeleteAppointment_ExistingId_CallsDeleteOnce()
        {
            var appointment = Appointment.Create(
                _doctor.Id, _patient.Id, ValidMorningTime, Shift.Morning);
            _appointmentsRepoMock.Setup(r => r.GetById(appointment.Id)).Returns(appointment);

            _service.DeleteAppointment(appointment.Id);

            _appointmentsRepoMock.Verify(r => r.Delete(appointment.Id), Times.Once);
        }

        [Test]
        public void DeleteAppointment_NotFound_ThrowsException()
        {
            var unknownId = Guid.NewGuid();
            _appointmentsRepoMock.Setup(r => r.GetById(unknownId)).Returns((Appointment)null);

            Assert.Throws<Exception>(() => _service.DeleteAppointment(unknownId));
        }

        // ─── GetAllByDoctor / GetAllByPatient ────────────────────────────

        [Test]
        public void GetAllByDoctor_CallsRepositoryWithCorrectDoctorId()
        {
            _appointmentsRepoMock
                .Setup(r => r.GetByDoctorId(_doctor.Id))
                .Returns(new List<Appointment>());

            _service.GetAllByDoctor(_doctor.Id);

            _appointmentsRepoMock.Verify(r => r.GetByDoctorId(_doctor.Id), Times.AtLeastOnce);
        }

        [Test]
        public void GetAllByPatient_CallsRepositoryWithCorrectPatientId()
        {
            _appointmentsRepoMock
                .Setup(r => r.GetByPatientId(_patient.Id))
                .Returns(new List<Appointment>());

            _service.GetAllByPatient(_patient.Id);

            _appointmentsRepoMock.Verify(r => r.GetByPatientId(_patient.Id), Times.AtLeastOnce);
        }
    }
}
