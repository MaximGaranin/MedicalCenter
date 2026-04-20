using System;

namespace MedicalCenter.Presentation.Contracts
{
    public record CreatePatientRequest(
        string FirstName,
        string LastName,
        string Patronymic,
        DateTime BirthDate,
        string Phone,
        string Address);

    public record PatientResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Patronymic,
        DateTime BirthDate,
        string Phone,
        string Address);
}
