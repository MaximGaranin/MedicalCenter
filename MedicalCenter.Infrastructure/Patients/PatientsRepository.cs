using System;
using System.Collections.Generic;
using System.Linq;
using MedicalCenter.Application.Patients.Abstractions;
using MedicalCenter.Domain.Patients;

namespace MedicalCenter.Infrastructure.Patients
{
    public sealed class PatientsRepository : IPatientsRepository
    {
        private readonly Dictionary<Guid, Patient> _patients = new();

        public void Add(Patient patient) => _patients[patient.Id] = patient;

        public void Delete(Guid id) => _patients.Remove(id);

        public Patient? GetById(Guid id) => _patients.GetValueOrDefault(id);

        public IReadOnlyList<Patient> GetAll() => _patients.Values.ToList();

        public IReadOnlyList<Patient> GetByLastName(string lastName) =>
            _patients.Values
                .Where(p => p.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase))
                .ToList();
    }
}
