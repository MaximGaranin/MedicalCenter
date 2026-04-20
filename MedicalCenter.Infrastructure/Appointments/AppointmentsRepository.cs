using System;
using System.Collections.Generic;
using System.Linq;
using MedicalCenter.Application.Appointments.Abstractions;
using MedicalCenter.Domain.Appointments;

namespace MedicalCenter.Infrastructure.Appointments
{
    public sealed class AppointmentsRepository : IAppointmentsRepository
    {
        private readonly Dictionary<Guid, Appointment> _appointments = new();

        public void Add(Appointment appointment) => _appointments[appointment.Id] = appointment;

        public void Delete(Guid id) => _appointments.Remove(id);

        public Appointment? GetById(Guid id) => _appointments.GetValueOrDefault(id);

        public IReadOnlyList<Appointment> GetByDoctorId(Guid doctorId) =>
            _appointments.Values.Where(a => a.DoctorId == doctorId).ToList();

        public IReadOnlyList<Appointment> GetByPatientId(Guid patientId) =>
            _appointments.Values.Where(a => a.PatientId == patientId).ToList();

        public IReadOnlyList<Appointment> GetByDoctorForWeek(Guid doctorId, DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(5);
            return _appointments.Values
                .Where(a => a.DoctorId == doctorId &&
                            a.AppointmentTime.Date >= weekStart.Date &&
                            a.AppointmentTime.Date < weekEnd.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToList();
        }

        public IReadOnlyList<Appointment> GetByDoctorForDate(Guid doctorId, DateTime date) =>
            _appointments.Values
                .Where(a => a.DoctorId == doctorId && a.AppointmentTime.Date == date.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToList();

        public IReadOnlyList<Appointment> GetByPatientForDate(Guid patientId, DateTime date) =>
            _appointments.Values
                .Where(a => a.PatientId == patientId && a.AppointmentTime.Date == date.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToList();
    }
}
