using System;
using MedicalCenter.Domain.Doctors;

namespace MedicalCenter.Presentation.Contracts
{
    public record CreateDoctorRequest(
        string FirstName,
        string LastName,
        string Patronymic,
        string Specialization,
        string Phone,
        Shift Shift);

    public record DoctorResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Patronymic,
        string Specialization,
        string Phone,
        string Shift,
        string ShiftTime);
}
