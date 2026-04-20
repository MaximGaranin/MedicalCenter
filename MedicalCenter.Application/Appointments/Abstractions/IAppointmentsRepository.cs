using System;
using System.Collections.Generic;
using MedicalCenter.Domain.Appointments;

namespace MedicalCenter.Application.Appointments.Abstractions
{
    public interface IAppointmentsRepository
    {
        void Add(Appointment appointment);
        void Delete(Guid id);
        Appointment? GetById(Guid id);
        IReadOnlyList<Appointment> GetByDoctorId(Guid doctorId);
        IReadOnlyList<Appointment> GetByPatientId(Guid patientId);
        IReadOnlyList<Appointment> GetByDoctorForWeek(Guid doctorId, DateTime weekStart);
        IReadOnlyList<Appointment> GetByDoctorForDate(Guid doctorId, DateTime date);
        IReadOnlyList<Appointment> GetByPatientForDate(Guid patientId, DateTime date);
    }
}
