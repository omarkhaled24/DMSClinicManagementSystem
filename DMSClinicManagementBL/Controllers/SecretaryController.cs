
using ClinicManagementBLL.ViewModels.PatientViewModel;
using ClinicManagemnetDAL.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagemnetDAL.Models;
using ClinicManagemnetDAL.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ClinicManagemnetDAL.Models.Enums;
using ClinicManagementBLL.ViewModels.AppointmentViewModels;
using Azure;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.Intrinsics.Arm;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Routing;

namespace DMSClinicManagementBL.Controllers
{


    namespace DMSClinicManagementBL.Controllers
    {
        public class SecretaryController : Controller
        {
            private readonly ClinicDbContext clinicDbContext;

            public SecretaryController(ClinicDbContext clinicDbContext)
            {
                this.clinicDbContext = clinicDbContext;
            }

            #region Create Appointment
            [HttpGet]
            public IActionResult CreateAppointment()
            {
                return View(new CreatePatient
                {
                    Doctors = clinicDbContext.Doctors.ToList()
                });
            }

            // ================== POST ==================
            // Rmote Validation Email 
            [AcceptVerbs("GET", "POST")]
            public IActionResult IsEmailUnique(string Email)
            {
                var exists = clinicDbContext.Patients
                    .Any(p => p.Email == Email);

                if (exists)
                    return Json("Email already exists");

                return Json(true);
            }

            // Rmote Validation Phone 
            [AcceptVerbs("GET", "POST")]
            public IActionResult IsPhoneUnique(string Phone)
            {
                var exists = clinicDbContext.Patients.Any(p => p.PhoneNumber == Phone);
                if (exists)
                    return Json($"Phone number {Phone} already exists");
                return Json(true);
            }


            [HttpPost]
            public IActionResult CreateAppointment(CreatePatient model)
            {
                // Unique validation
                if (clinicDbContext.Patients.Any(p => p.Email == model.Email))
                    ModelState.AddModelError("Email", "Email already exists");

                if (clinicDbContext.Patients.Any(p => p.PhoneNumber == model.Phone))
                    ModelState.AddModelError("Phone", "Phone already exists");

                if (model.AppointmentDate.Date < DateTime.Today)
                {
                    ModelState.AddModelError("AppointmentDate", "Cannot book appointment in the past");
                }

                if (!ModelState.IsValid)
                {
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }

                // Address
                var address = clinicDbContext.Addresses.FirstOrDefault(a =>
                    a.BuildingNumber == model.BuildingNumber &&
                    a.FloorNumber == model.FloorNumber &&
                    a.Street == model.Street &&
                    a.City == model.City);

                if (address == null)
                {
                    address = new Address
                    {
                        BuildingNumber = model.BuildingNumber,
                        FloorNumber = model.FloorNumber,
                        Street = model.Street,
                        City = model.City
                    };
                    clinicDbContext.Add(address);
                    clinicDbContext.SaveChanges();
                }

                // Patient
                if (model.DateOfBirth == null)
                {
                    ModelState.AddModelError("DateOfBirth", "Date of Birth is required");
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }
                var patient = new Patient
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    DateOfBirth = model.DateOfBirth.Value,
                    Gender = model.Gender,
                    AddressId = address.Id
                };
                clinicDbContext.Patients.Add(patient);
                clinicDbContext.SaveChanges();

                // Doctor
                var doctor = clinicDbContext.Doctors.FirstOrDefault(d => d.Id == model.DoctorId);
                if (doctor == null)
                {
                    ModelState.AddModelError("", "Invalid Doctor");
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }

                // Time
                if (!TimeSpan.TryParse(model.SelectedTime, out var startTime))
                {
                    ModelState.AddModelError("", "Invalid Time");
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }

                // Prevent double booking
                bool booked = clinicDbContext.Appointments.Any(a =>
                    a.DoctorId == model.DoctorId &&
                    a.AppointmentDate == model.AppointmentDate.Date &&
                    a.StartTime == startTime);

                if (booked)
                {
                    ModelState.AddModelError("", "This time is already booked");
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }
                var secretary = clinicDbContext.Secretaries.FirstOrDefault();
                var appointment = new Appointment
                {
                    DoctorId = doctor.Id,
                    PatientId = patient.Id,
                    SecretaryId = secretary.Id,
                    AppointmentDate = model.AppointmentDate.Date,
                    StartTime = startTime,
                    EndTime = startTime.Add(TimeSpan.FromMinutes(30))
                };

                clinicDbContext.Appointments.Add(appointment);
                clinicDbContext.SaveChanges();


                var newAppointment = clinicDbContext.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Include(a => a.Secretary)
                    .FirstOrDefault(a => a.Id == appointment.Id);

                return View("AppointmentDetails", newAppointment);
            }


            // ================== AJAX ==================
            [HttpGet]
            public IActionResult GetAvailableSlots(int doctorId, string date)
            {
                if (!DateTime.TryParse(date, out var day))
                    return Json(new { isOff = true, message = "Invalid Date" });

                var dayName = day.DayOfWeek.ToString();

                var schedule = clinicDbContext.Schedules
                    .FirstOrDefault(s => s.DoctorId == doctorId && s.DayOfWeek == dayName);

                if (schedule == null || !schedule.IsWorking)
                {
                    return Json(new { isOff = true, message = "Doctor is off on this day" });
                }

                var slots = new List<string>();

                for (var t = schedule.StartTime; t < schedule.EndTime; t = t.Add(TimeSpan.FromMinutes(30)))
                {
                    bool booked = clinicDbContext.Appointments.Any(a =>
                        a.DoctorId == doctorId &&
                        a.AppointmentDate == day.Date &&
                        a.StartTime == t);

                    if (!booked)
                        slots.Add(t.ToString(@"hh\:mm"));
                }

                return Json(new { isOff = false, slots = slots });
            }

            #endregion

            #region Get Patient Data
            [HttpGet]
            public IActionResult PatientsWithAppointments()
            {
                var data = clinicDbContext.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .OrderBy(a => a.AppointmentDate)
                    .Select(a => new PatientAppointmentList
                    {
                        PatientId = a.Patient.Id,
                        PatientName = a.Patient.Name,
                        DoctorName = a.Doctor.Name,
                        Phone = a.Patient.PhoneNumber,
                        AppointmentDate = a.AppointmentDate
                    })
                    .ToList();

                return View(data);
            }
            [HttpGet]
            public IActionResult PatientDetails(int id)
            {
                if (id <= 0)
                    return RedirectToAction(nameof(PatientsWithAppointments));

                var appointment = clinicDbContext.Appointments
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.Address)
                    .Include(a => a.Doctor)
                    .FirstOrDefault(a => a.PatientId == id);

                if (appointment == null)
                    return RedirectToAction(nameof(PatientsWithAppointments));

                var model = new PatientDetailsViewModel
                {
                    Id = appointment.Patient.Id,  // ← مهم جدًا
                    Name = appointment.Patient.Name,
                    Doctor = appointment.Doctor.Name,
                    Phone = appointment.Patient.PhoneNumber,
                    Email = appointment.Patient.Email,
                    BuildingNumber = appointment.Patient.Address?.BuildingNumber ?? "",
                    FloorNumber = appointment.Patient.Address?.FloorNumber ?? "",
                    Street = appointment.Patient.Address?.Street ?? "",
                    City = appointment.Patient.Address?.City ?? "",
                    Age = DateTime.Today.Year - appointment.Patient.DateOfBirth.Year -
                          (DateTime.Today.DayOfYear < appointment.Patient.DateOfBirth.DayOfYear ? 1 : 0),
                    AppointmentDate = appointment.AppointmentDate,
                    StartTime = appointment.StartTime
                };

                return View(model);
            }

            //[HttpGet]
            //public IActionResult PatientDetails(int id)
            //{
            //    if (id <= 0)
            //        return RedirectToAction(nameof(PatientsWithAppointments));

            //    var appointment = clinicDbContext.Appointments
            //        .Include(a => a.Patient)
            //            .ThenInclude(p => p.Address)
            //        .Include(a => a.Doctor)
            //        .FirstOrDefault(a => a.PatientId == id);

            //    if (appointment == null)
            //        return RedirectToAction(nameof(PatientsWithAppointments));

            //    var model = new PatientDetailsViewModel
            //    {

            //        Name = appointment.Patient.Name,
            //        Doctor = appointment.Doctor.Name,
            //        Phone = appointment.Patient.PhoneNumber,
            //        Email = appointment.Patient.Email,
            //        BuildingNumber = appointment.Patient.Address?.BuildingNumber ?? "",
            //        FloorNumber = appointment.Patient.Address?.FloorNumber ?? "",
            //        Street = appointment.Patient.Address?.Street ?? "",
            //        City = appointment.Patient.Address?.City ?? "",

            //        Age = DateTime.Today.Year - appointment.Patient.DateOfBirth.Year -
            //              (DateTime.Today.DayOfYear < appointment.Patient.DateOfBirth.DayOfYear ? 1 : 0),

            //        AppointmentDate = appointment.AppointmentDate,
            //        StartTime = appointment.StartTime
            //    };

            //    return View(model);
            //}

            #endregion

            #region Edit Patient
            [HttpGet]
            public IActionResult EditPatient(int id)
            {
                var appointment = clinicDbContext.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Patient.Address)
                    .FirstOrDefault(a => a.PatientId == id);

                if (appointment == null)
                    return RedirectToAction(nameof(PatientsWithAppointments));

                var model = new PatientEditViewModel
                {
                    Id = appointment.Patient.Id,
                    Name = appointment.Patient.Name,
                    Email = appointment.Patient.Email,
                    Phone = appointment.Patient.PhoneNumber,
                    BuildingNumber = appointment.Patient.Address?.BuildingNumber ?? "",
                    FloorNumber = appointment.Patient.Address?.FloorNumber ?? "",
                    Street = appointment.Patient.Address?.Street ?? "",
                    City = appointment.Patient.Address?.City ?? "",

                    DoctorId = appointment.DoctorId,
                    Doctors = clinicDbContext.Doctors.ToList(),

                    AppointmentDate = appointment.AppointmentDate,
                    SelectedTime = appointment.StartTime.ToString(@"hh\:mm")
                };

                return View(model);
            }

            [HttpPost]
            public IActionResult EditPatient(PatientEditViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }
                if (model.AppointmentDate.Date < DateTime.Today)
                {
                    ModelState.AddModelError("AppointmentDate", "You cannot select a past date.");
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }

                var appointment = clinicDbContext.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Patient.Address)
                    .FirstOrDefault(a => a.PatientId == model.Id);

                if (appointment == null)
                    return RedirectToAction(nameof(PatientsWithAppointments));

                // تحديث بيانات المريض
                appointment.Patient.Name = model.Name;
                appointment.Patient.Email = model.Email;
                appointment.Patient.PhoneNumber = model.Phone;

                if (appointment.Patient.Address == null)
                    appointment.Patient.Address = new Address();

                appointment.Patient.Address.BuildingNumber = model.BuildingNumber;
                appointment.Patient.Address.FloorNumber = model.FloorNumber;
                appointment.Patient.Address.Street = model.Street;
                appointment.Patient.Address.City = model.City;

                // 🔥 تحديث الحجز
                appointment.DoctorId = model.DoctorId;
                appointment.AppointmentDate = model.AppointmentDate.Date;

                if (TimeSpan.TryParse(model.SelectedTime, out var startTime))
                {
                    appointment.StartTime = startTime;
                    appointment.EndTime = startTime.Add(TimeSpan.FromMinutes(30));
                }

                clinicDbContext.SaveChanges();

                return RedirectToAction(nameof(PatientDetails), new { id = model.Id });
            }

            #endregion

            #region Delete Patient 
            [HttpPost]
            public IActionResult DeletePatient(int id)
            {
                var patient = clinicDbContext.Patients
                    .Include(p => p.Appointments)
                    .FirstOrDefault(p => p.Id == id);

                if (patient == null)
                    return RedirectToAction(nameof(PatientsWithAppointments));

                if (patient.Appointments.Any())
                {
                    clinicDbContext.Appointments.RemoveRange(patient.Appointments);
                }

                clinicDbContext.Patients.Remove(patient);
                clinicDbContext.SaveChanges();

                return RedirectToAction(nameof(PatientsWithAppointments));
            }

            #endregion
        }

    }
}


