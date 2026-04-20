using System;
using NUnit.Framework;
using MedicalCenter.Domain.Appointments;
using MedicalCenter.Domain.Doctors;

namespace MedicalCenter.Tests.Domain
{
    [TestFixture]
    public class AppointmentTests
    {
        private static readonly Guid DoctorId  = Guid.NewGuid();
        private static readonly Guid PatientId = Guid.NewGuid();

        // Ближайший понедельник для стабильных тестов
        private static DateTime NextMonday()
        {
            var date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Monday)
                date = date.AddDays(1);
            return date;
        }

        // ─── Позитивные тесты ───────────────────────────────────────────

        [TestCase(8,  0,  Shift.Morning)]
        [TestCase(8,  30, Shift.Morning)]
        [TestCase(13, 30, Shift.Morning)]
        [TestCase(14, 0,  Shift.Evening)]
        [TestCase(14, 30, Shift.Evening)]
        [TestCase(19, 30, Shift.Evening)]
        public void Create_ValidTime_ReturnsAppointment(int hour, int minute, Shift shift)
        {
            var time = NextMonday().Date.AddHours(hour).AddMinutes(minute);

            var appointment = Appointment.Create(DoctorId, PatientId, time, shift);

            Assert.IsNotNull(appointment);
            Assert.That(appointment.AppointmentTime, Is.EqualTo(time));
        }

        [Test]
        public void Create_ValidAppointment_EndTimeIs30MinutesLater()
        {
            var time = NextMonday().Date.AddHours(10);
            var appointment = Appointment.Create(DoctorId, PatientId, time, Shift.Morning);

            Assert.That(appointment.EndTime, Is.EqualTo(time.AddMinutes(30)));
        }

        [Test]
        public void Create_TwoAppointments_HaveDifferentIds()
        {
            var time1 = NextMonday().Date.AddHours(8);
            var time2 = NextMonday().Date.AddHours(9);

            var a1 = Appointment.Create(DoctorId, PatientId, time1, Shift.Morning);
            var a2 = Appointment.Create(DoctorId, PatientId, time2, Shift.Morning);

            Assert.That(a1.Id, Is.Not.EqualTo(a2.Id));
        }

        // ─── Негативные тесты: выходные ─────────────────────────────────

        [Test]
        public void Create_Saturday_ThrowsException()
        {
            var saturday = NextMonday().AddDays(5).Date.AddHours(10);

            var ex = Assert.Throws<Exception>(() =>
                Appointment.Create(DoctorId, PatientId, saturday, Shift.Morning));

            Assert.That(ex.Message, Is.EqualTo("Приём ведётся только с понедельника по пятницу"));
        }

        [Test]
        public void Create_Sunday_ThrowsException()
        {
            var sunday = NextMonday().AddDays(6).Date.AddHours(10);

            var ex = Assert.Throws<Exception>(() =>
                Appointment.Create(DoctorId, PatientId, sunday, Shift.Morning));

            Assert.That(ex.Message, Is.EqualTo("Приём ведётся только с понедельника по пятницу"));
        }

        // ─── Негативные тесты: время не кратно 30 мин ──────────────────

        [TestCase(8,  15)]
        [TestCase(9,  10)]
        [TestCase(10, 45)]
        public void Create_TimeNotMultipleOf30_ThrowsException(int hour, int minute)
        {
            var time = NextMonday().Date.AddHours(hour).AddMinutes(minute);

            var ex = Assert.Throws<Exception>(() =>
                Appointment.Create(DoctorId, PatientId, time, Shift.Morning));

            Assert.That(ex.Message, Does.Contain("кратное 30 минутам"));
        }

        // ─── Негативные тесты: вне диапазона смены ──────────────────────

        [TestCase(7,  30)]
        [TestCase(14, 0)]   // начало вечерней смены — для утреннего врача это ошибка
        [TestCase(20, 0)]
        public void Create_MorningShift_OutOfRange_ThrowsException(int hour, int minute)
        {
            var time = NextMonday().Date.AddHours(hour).AddMinutes(minute);

            var ex = Assert.Throws<Exception>(() =>
                Appointment.Create(DoctorId, PatientId, time, Shift.Morning));

            Assert.That(ex.Message, Does.Contain("утренней смены"));
        }

        [TestCase(8,  0)]   // начало утренней — для вечернего врача ошибка
        [TestCase(13, 30)]
        [TestCase(20, 0)]
        public void Create_EveningShift_OutOfRange_ThrowsException(int hour, int minute)
        {
            var time = NextMonday().Date.AddHours(hour).AddMinutes(minute);

            var ex = Assert.Throws<Exception>(() =>
                Appointment.Create(DoctorId, PatientId, time, Shift.Evening));

            Assert.That(ex.Message, Does.Contain("вечерней смены"));
        }
    }
}
