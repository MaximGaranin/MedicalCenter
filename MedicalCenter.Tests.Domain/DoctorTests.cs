using System;
using NUnit.Framework;
using MedicalCenter.Domain.Doctors;

namespace MedicalCenter.Tests.Domain
{
    [TestFixture]
    public class DoctorTests
    {
        // ─── Позитивные тесты ───────────────────────────────────────────

        [Test]
        public void Create_ValidData_ReturnsDoctor()
        {
            var doctor = Doctor.Create("Пётр", "Смирнов", "Алексеевич",
                "Терапевт", "+79005551234", Shift.Morning);

            Assert.IsNotNull(doctor);
        }

        [TestCase("Пётр",   "Смирнов",  "Терапевт",   Shift.Morning)]
        [TestCase("Анна",   "Козлова",  "Хирург",     Shift.Evening)]
        [TestCase("Олег",   "Новиков",  "Кардиолог",  Shift.Morning)]
        public void Create_ValidData_SetsPropertiesCorrectly(
            string firstName, string lastName, string specialization, Shift shift)
        {
            var doctor = Doctor.Create(firstName, lastName, "Отчество",
                specialization, "79009999999", shift);

            Assert.That(doctor.FirstName,       Is.EqualTo(firstName));
            Assert.That(doctor.LastName,        Is.EqualTo(lastName));
            Assert.That(doctor.Specialization,  Is.EqualTo(specialization));
            Assert.That(doctor.Shift,           Is.EqualTo(shift));
            Assert.That(doctor.Id,              Is.Not.EqualTo(Guid.Empty));
        }

        // ─── Негативные тесты ───────────────────────────────────────────

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Create_InvalidFirstName_ThrowsException(string invalidName)
        {
            var ex = Assert.Throws<Exception>(() =>
                Doctor.Create(invalidName, "Смирнов", "", "Терапевт", "", Shift.Morning));

            Assert.That(ex.Message, Is.EqualTo("Имя врача не может быть пустым"));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Create_InvalidLastName_ThrowsException(string invalidLastName)
        {
            var ex = Assert.Throws<Exception>(() =>
                Doctor.Create("Пётр", invalidLastName, "", "Терапевт", "", Shift.Morning));

            Assert.That(ex.Message, Is.EqualTo("Фамилия врача не может быть пустой"));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Create_InvalidSpecialization_ThrowsException(string invalidSpec)
        {
            var ex = Assert.Throws<Exception>(() =>
                Doctor.Create("Пётр", "Смирнов", "", invalidSpec, "", Shift.Morning));

            Assert.That(ex.Message, Is.EqualTo("Специализация не может быть пустой"));
        }
    }
}
