using System;

namespace MedicalCenter.Domain.Doctors
{
    public sealed class Doctor
    {
        private Doctor(
            Guid id,
            string firstName,
            string lastName,
            string patronymic,
            string specialization,
            string phone,
            Shift shift)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Specialization = specialization;
            Phone = phone;
            Shift = shift;
        }

        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Patronymic { get; private set; }
        public string Specialization { get; private set; }
        public string Phone { get; private set; }
        public Shift Shift { get; private set; }

        public static Doctor Create(
            string firstName,
            string lastName,
            string patronymic,
            string specialization,
            string phone,
            Shift shift)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new Exception("Имя врача не может быть пустым");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new Exception("Фамилия врача не может быть пустой");
            if (string.IsNullOrWhiteSpace(specialization))
                throw new Exception("Специализация не может быть пустой");

            return new Doctor(Guid.NewGuid(), firstName, lastName, patronymic, specialization, phone, shift);
        }
    }
}
