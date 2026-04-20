using System;
using System.Collections.Generic;
using System.Linq;
using MedicalCenter.Application.Appointments.Abstractions;
using MedicalCenter.Application.Doctors.Abstractions;
using MedicalCenter.Application.Patients.Abstractions;
using MedicalCenter.Domain.Appointments;

namespace MedicalCenter.Application.Appointments
{
    public sealed class AppointmentsService
    {
        private readonly IAppointmentsRepository _appointmentsRepo;
        private readonly IDoctorsRepository _doctorsRepo;
        private readonly IPatientsRepository _patientsRepo;

        public AppointmentsService(
            IAppointmentsRepository appointmentsRepo,
            IDoctorsRepository doctorsRepo,
            IPatientsRepository patientsRepo)
        {
            _appointmentsRepo = appointmentsRepo;
            _doctorsRepo = doctorsRepo;
            _patientsRepo = patientsRepo;
        }

        public Guid CreateAppointment(Guid doctorId, Guid patientId, DateTime appointmentTime)
        {
            var doctor = _doctorsRepo.GetById(doctorId);
            if (doctor == null)
                throw new Exception($"Врач с id={doctorId} не найден");

            var patient = _patientsRepo.GetById(patientId);
            if (patient == null)
                throw new Exception($"Пациент с id={patientId} не найден");

            // Проверка: врач уже занят в это время
            var doctorAppointments = _appointmentsRepo.GetByDoctorId(doctorId);
            bool doctorBusy = doctorAppointments.Any(a =>
                a.AppointmentTime == appointmentTime);
            if (doctorBusy)
                throw new Exception($"Врач {doctor.LastName} уже занят в {appointmentTime:HH:mm}");

            // Проверка: пациент уже записан к другому врачу в это время
            var patientAppointments = _appointmentsRepo.GetByPatientId(patientId);
            bool patientBusy = patientAppointments.Any(a =>
                a.AppointmentTime == appointmentTime);
            if (patientBusy)
                throw new Exception($"Пациент {patient.LastName} уже записан на {appointmentTime:HH:mm}");

            var appointment = Appointment.Create(doctorId, patientId, appointmentTime, doctor.Shift);
            _appointmentsRepo.Add(appointment);
            return appointment.Id;
        }

        public void DeleteAppointment(Guid id)
        {
            var appointment = _appointmentsRepo.GetById(id);
            if (appointment == null)
                throw new Exception($"Запись с id={id} не найдена");
            _appointmentsRepo.Delete(id);
        }

        public IReadOnlyList<Appointment> GetWeekScheduleForDoctor(Guid doctorId, DateTime weekStart) =>
            _appointmentsRepo.GetByDoctorForWeek(doctorId, weekStart);

        public IReadOnlyList<Appointment> GetTodayAppointmentsForDoctor(Guid doctorId) =>
            _appointmentsRepo.GetByDoctorForDate(doctorId, DateTime.Today);

        public IReadOnlyList<Appointment> GetTodayAppointmentsForPatient(Guid patientId) =>
            _appointmentsRepo.GetByPatientForDate(patientId, DateTime.Today);

        public IReadOnlyList<Appointment> GetAllByDoctor(Guid doctorId) =>
            _appointmentsRepo.GetByDoctorId(doctorId);

        public IReadOnlyList<Appointment> GetAllByPatient(Guid patientId) =>
            _appointmentsRepo.GetByPatientId(patientId);
    }
}
