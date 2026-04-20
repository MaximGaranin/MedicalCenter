using System;
using System.Collections.Generic;
using System.Linq;
using MedicalCenter.Application.Doctors.Abstractions;
using MedicalCenter.Domain.Doctors;

namespace MedicalCenter.Infrastructure.Doctors
{
    public sealed class DoctorsRepository : IDoctorsRepository
    {
        private readonly Dictionary<Guid, Doctor> _doctors = new();

        public void Add(Doctor doctor) => _doctors[doctor.Id] = doctor;

        public void Delete(Guid id) => _doctors.Remove(id);

        public Doctor? GetById(Guid id) => _doctors.GetValueOrDefault(id);

        public IReadOnlyList<Doctor> GetAll() => _doctors.Values.ToList();

        public IReadOnlyList<Doctor> GetByLastName(string lastName) =>
            _doctors.Values
                .Where(d => d.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase))
                .ToList();
    }
}
