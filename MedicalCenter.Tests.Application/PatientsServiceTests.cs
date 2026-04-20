using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using MedicalCenter.Application.Patients;
using MedicalCenter.Application.Patients.Abstractions;
using MedicalCenter.Domain.Patients;

namespace MedicalCenter.Tests.Application
{
    [TestFixture]
    public class PatientsServiceTests
    {
        private Mock<IPatientsRepository> _repoMock;
        private PatientsService           _service;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IPatientsRepository>();
            _service  = new PatientsService(_repoMock.Object);
        }

        // ─── CreatePatient ───────────────────────────────────────────────

        [Test]
        public void CreatePatient_ValidData_CallsRepositoryAddOnce()
        {
            _service.CreatePatient("Иван", "Иванов", "Иванович",
                new DateTime(1990, 1, 1), "79001234567", "Москва");

            _repoMock.Verify(r => r.Add(It.IsAny<Patient>()), Times.Once);
        }

        [Test]
        public void CreatePatient_ValidData_ReturnsNonEmptyGuid()
        {
            var id = _service.CreatePatient("Иван", "Иванов", "",
                new DateTime(1990, 1, 1), "", "");

            Assert.That(id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void CreatePatient_AddedPatientHasCorrectName()
        {
            Patient addedPatient = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Patient>()))
                     .Callback<Patient>(p => addedPatient = p);

            _service.CreatePatient("Иван", "Иванов", "",
                new DateTime(1990, 1, 1), "", "");

            Assert.That(addedPatient.FirstName, Is.EqualTo("Иван"));
            Assert.That(addedPatient.LastName,  Is.EqualTo("Иванов"));
        }

        // ─── GetPatient ──────────────────────────────────────────────────

        [Test]
        public void GetPatient_ExistingId_ReturnsPatient()
        {
            var expected = Patient.Create("Мария", "Петрова", "",
                new DateTime(1985, 6, 15), "", "");
            _repoMock.Setup(r => r.GetById(expected.Id)).Returns(expected);

            var result = _service.GetPatient(expected.Id);

            Assert.That(result, Is.EqualTo(expected));
            _repoMock.Verify(r => r.GetById(expected.Id), Times.Once);
        }

        [Test]
        public void GetPatient_NotFound_ThrowsException()
        {
            var unknownId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetById(unknownId)).Returns((Patient)null);

            var ex = Assert.Throws<Exception>(() => _service.GetPatient(unknownId));

            Assert.That(ex.Message, Does.Contain(unknownId.ToString()));
        }

        // ─── DeletePatient ───────────────────────────────────────────────

        [Test]
        public void DeletePatient_ExistingId_CallsRepositoryDeleteOnce()
        {
            var patient = Patient.Create("Алексей", "Сидоров", "",
                new DateTime(1975, 3, 20), "", "");
            _repoMock.Setup(r => r.GetById(patient.Id)).Returns(patient);

            _service.DeletePatient(patient.Id);

            _repoMock.Verify(r => r.Delete(patient.Id), Times.Once);
        }

        [Test]
        public void DeletePatient_NotFound_ThrowsException()
        {
            var unknownId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetById(unknownId)).Returns((Patient)null);

            Assert.Throws<Exception>(() => _service.DeletePatient(unknownId));
        }

        // ─── GetAllPatients ──────────────────────────────────────────────

        [Test]
        public void GetAllPatients_ReturnsListFromRepository()
        {
            var list = new List<Patient>
            {
                Patient.Create("А", "Б", "", new DateTime(1990, 1, 1), "", ""),
                Patient.Create("В", "Г", "", new DateTime(1992, 2, 2), "", "")
            };
            _repoMock.Setup(r => r.GetAll()).Returns(list);

            var result = _service.GetAllPatients();

            Assert.That(result.Count, Is.EqualTo(2));
            _repoMock.Verify(r => r.GetAll(), Times.Once);
        }

        // ─── SearchByLastName ────────────────────────────────────────────

        [Test]
        public void SearchByLastName_CallsRepositoryWithCorrectLastName()
        {
            _repoMock.Setup(r => r.GetByLastName("Иванов"))
                     .Returns(new List<Patient>());

            _service.SearchByLastName("Иванов");

            _repoMock.Verify(r => r.GetByLastName("Иванов"), Times.Once);
        }
    }
}
