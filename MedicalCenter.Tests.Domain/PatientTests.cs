using System;
using NUnit.Framework;
using MedicalCenter.Domain.Patients;

namespace MedicalCenter.Tests.Domain
{
    [TestFixture]
    public class PatientTests
    {
        [Test]
        public void Create_ValidData_ReturnsPatient()
        {
            var patient = Patient.Create("Иван", "Иванов", "Иванович",
                new DateTime(1990, 5, 15), "+79001234567", "Москва, ул. Ленина 1");

            Assert.IsNotNull(patient);
        }

        [TestCase("Иван",    "Иванов",   "Иванович",   1990, 5, 15)]
        [TestCase("Мария",   "Петрова",  "Сергеевна",  2000, 1,  1)]
        [TestCase("Алексей", "Сидоров",  "",           1985, 12, 31)]
        public void Create_ValidData_SetsPropertiesCorrectly(
            string firstName, string lastName, string patronymic,
            int year, int month, int day)
        {
            var birthDate = new DateTime(year, month, day);
            var patient = Patient.Create(firstName, lastName, patronymic,
                birthDate, "79001234567", "Адрес");

            Assert.That(patient.FirstName,  Is.EqualTo(firstName));
            Assert.That(patient.LastName,   Is.EqualTo(lastName));
            Assert.That(patient.Patronymic, Is.EqualTo(patronymic));
            Assert.That(patient.BirthDate,  Is.EqualTo(birthDate));
            Assert.That(patient.Id,         Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void Create_TwoPatients_HaveDifferentIds()
        {
            var p1 = Patient.Create("Иван",  "Иванов",  "", new DateTime(1990, 1, 1), "", "");
            var p2 = Patient.Create("Мария", "Петрова", "", new DateTime(1992, 2, 2), "", "");

            Assert.That(p1.Id, Is.Not.EqualTo(p2.Id));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Create_InvalidFirstName_ThrowsException(string invalidName)
        {
            var ex = Assert.Throws<Exception>(() =>
                Patient.Create(invalidName, "Иванов", "Иванович",
                    new DateTime(1990, 1, 1), "", ""));

            Assert.That(ex.Message, Is.EqualTo("Имя пациента не может быть пустым"));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Create_InvalidLastName_ThrowsException(string invalidLastName)
        {
            var ex = Assert.Throws<Exception>(() =>
                Patient.Create("Иван", invalidLastName, "Иванович",
                    new DateTime(1990, 1, 1), "", ""));

            Assert.That(ex.Message, Is.EqualTo("Фамилия пациента не может быть пустой"));
        }

        [Test]
        public void Create_FutureBirthDate_ThrowsException()
        {
            var futureDate = DateTime.Today.AddDays(1);

            var ex = Assert.Throws<Exception>(() =>
                Patient.Create("Иван", "Иванов", "", futureDate, "", ""));

            Assert.That(ex.Message, Is.EqualTo("Дата рождения не может быть в будущем"));
        }

        [Test]
        public void Create_TodayBirthDate_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                Patient.Create("Иван", "Иванов", "", DateTime.Today, "", ""));
        }
    }
}
