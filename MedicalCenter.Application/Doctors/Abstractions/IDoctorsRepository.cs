using System;
using System.Collections.Generic;
using MedicalCenter.Domain.Doctors;

namespace MedicalCenter.Application.Doctors.Abstractions
{
    public interface IDoctorsRepository
    {
        void Add(Doctor doctor);
        void Delete(Guid id);
        Doctor? GetById(Guid id);
        IReadOnlyList<Doctor> GetAll();
        IReadOnlyList<Doctor> GetByLastName(string lastName);
    }
}
