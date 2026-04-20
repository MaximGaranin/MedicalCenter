using System;
using System.Collections.Generic;
using MedicalCenter.Application.Doctors.Abstractions;
using MedicalCenter.Domain.Doctors;

namespace MedicalCenter.Application.Doctors
{
    public sealed class DoctorsService
    {
        private readonly IDoctorsRepository _repository;

        public DoctorsService(IDoctorsRepository repository)
        {
            _repository = repository;
        }

        public Guid CreateDoctor(
            string firstName,
            string lastName,
            string patronymic,
            string specialization,
            string phone,
            Shift shift)
        {
            var doctor = Doctor.Create(firstName, lastName, patronymic, specialization, phone, shift);
            _repository.Add(doctor);
            return doctor.Id;
        }

        public void DeleteDoctor(Guid id)
        {
            var doctor = _repository.GetById(id);
            if (doctor == null)
                throw new Exception($"Врач с id={id} не найден");
            _repository.Delete(id);
        }

        public Doctor GetDoctor(Guid id)
        {
            var doctor = _repository.GetById(id);
            if (doctor == null)
                throw new Exception($"Врач с id={id} не найден");
            return doctor;
        }

        public IReadOnlyList<Doctor> GetAllDoctors() => _repository.GetAll();

        public IReadOnlyList<Doctor> SearchByLastName(string lastName) =>
            _repository.GetByLastName(lastName);
    }
}
