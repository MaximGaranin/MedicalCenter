using System;

namespace MedicalCenter.Domain.Patients
{
    public sealed class Patient
    {
        private Patient(
            Guid id,
            string firstName,
            string lastName,
            string patronymic,
            DateTime birthDate,
            string phone,
            string address)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            BirthDate = birthDate;
            Phone = phone;
            Address = address;
        }

        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Patronymic { get; private set; }
        public DateTime BirthDate { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }

        public static Patient Create(
            string firstName,
            string lastName,
            string patronymic,
            DateTime birthDate,
            string phone,
            string address)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new Exception("Имя пациента не может быть пустым");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new Exception("Фамилия пациента не может быть пустой");
            if (birthDate > DateTime.Today)
                throw new Exception("Дата рождения не может быть в будущем");

            return new Patient(Guid.NewGuid(), firstName, lastName, patronymic, birthDate, phone, address);
        }
    }
}
