using System;
using System.Collections.Generic;
using MedicalCenter.Application.Patients.Abstractions;
using MedicalCenter.Domain.Patients;

namespace MedicalCenter.Application.Patients
{
    public sealed class PatientsService
    {
        private readonly IPatientsRepository _repository;

        public PatientsService(IPatientsRepository repository)
        {
            _repository = repository;
        }

        public Guid CreatePatient(
            string firstName,
            string lastName,
            string patronymic,
            DateTime birthDate,
            string phone,
            string address)
        {
            var patient = Patient.Create(firstName, lastName, patronymic, birthDate, phone, address);
            _repository.Add(patient);
            return patient.Id;
        }

        public void DeletePatient(Guid id)
        {
            var patient = _repository.GetById(id);
            if (patient == null)
                throw new Exception($"Пациент с id={id} не найден");
            _repository.Delete(id);
        }

        public Patient GetPatient(Guid id)
        {
            var patient = _repository.GetById(id);
            if (patient == null)
                throw new Exception($"Пациент с id={id} не найден");
            return patient;
        }

        public IReadOnlyList<Patient> GetAllPatients() => _repository.GetAll();

        public IReadOnlyList<Patient> SearchByLastName(string lastName) =>
            _repository.GetByLastName(lastName);
    }
}
