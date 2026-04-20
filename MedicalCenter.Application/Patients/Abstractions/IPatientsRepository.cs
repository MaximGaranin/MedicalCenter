using System;
using System.Collections.Generic;
using MedicalCenter.Domain.Patients;

namespace MedicalCenter.Application.Patients.Abstractions
{
    public interface IPatientsRepository
    {
        void Add(Patient patient);
        void Delete(Guid id);
        Patient? GetById(Guid id);
        IReadOnlyList<Patient> GetAll();
        IReadOnlyList<Patient> GetByLastName(string lastName);
    }
}
