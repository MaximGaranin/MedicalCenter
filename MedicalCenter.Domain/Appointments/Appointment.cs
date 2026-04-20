using System;

namespace MedicalCenter.Domain.Appointments
{
    public sealed class Appointment
    {
        private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan MorningStart = new TimeSpan(8, 0, 0);
        private static readonly TimeSpan MorningEnd = new TimeSpan(14, 0, 0);
        private static readonly TimeSpan EveningStart = new TimeSpan(14, 0, 0);
        private static readonly TimeSpan EveningEnd = new TimeSpan(20, 0, 0);

        private Appointment(
            Guid id,
            Guid doctorId,
            Guid patientId,
            DateTime appointmentTime)
        {
            Id = id;
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentTime = appointmentTime;
        }

        public Guid Id { get; private set; }
        public Guid DoctorId { get; private set; }
        public Guid PatientId { get; private set; }
        public DateTime AppointmentTime { get; private set; }
        public DateTime EndTime => AppointmentTime.Add(SlotDuration);

        public static Appointment Create(
            Guid doctorId,
            Guid patientId,
            DateTime appointmentTime,
            Doctors.Shift doctorShift)
        {
            if (appointmentTime.DayOfWeek == DayOfWeek.Saturday || appointmentTime.DayOfWeek == DayOfWeek.Sunday)
                throw new Exception("Приём ведётся только с понедельника по пятницу");

            var time = appointmentTime.TimeOfDay;

            if (doctorShift == Doctors.Shift.Morning)
            {
                if (time < MorningStart || time >= MorningEnd - SlotDuration + TimeSpan.FromMinutes(1))
                    throw new Exception("Время записи выходит за пределы утренней смены (08:00–14:00)");
            }
            else
            {
                if (time < EveningStart || time >= EveningEnd - SlotDuration + TimeSpan.FromMinutes(1))
                    throw new Exception("Время записи выходит за пределы вечерней смены (14:00–20:00)");
            }

            if (appointmentTime.Minute % 30 != 0)
                throw new Exception("Запись возможна только на время кратное 30 минутам (08:00, 08:30, ...)");

            return new Appointment(Guid.NewGuid(), doctorId, patientId, appointmentTime);
        }
    }
}
