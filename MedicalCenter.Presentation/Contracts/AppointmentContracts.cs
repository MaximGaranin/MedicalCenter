using System;

namespace MedicalCenter.Presentation.Contracts
{
    public record CreateAppointmentRequest(
        Guid DoctorId,
        Guid PatientId,
        DateTime AppointmentTime);

    public record AppointmentResponse(
        Guid Id,
        Guid DoctorId,
        string DoctorFullName,
        Guid PatientId,
        string PatientFullName,
        DateTime AppointmentTime,
        DateTime EndTime);
}
