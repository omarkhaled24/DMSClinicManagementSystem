
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
            [HttpGet]
            public IActionResult CreateAppointment()
            {
                return View(new CreatePatient
                {
                    Doctors = clinicDbContext.Doctors.ToList()
                });
            }

            // ================== POST ==================
            [HttpPost]
            public IActionResult CreateAppointment(CreatePatient model)
            {
                // Unique validation
                if (clinicDbContext.Patients.Any(p => p.Email == model.Email))
                    ModelState.AddModelError("Email", "Email already exists");

                if (clinicDbContext.Patients.Any(p => p.PhoneNumber == model.Phone))
                    ModelState.AddModelError("Phone", "Phone already exists");

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
                var patient = new Patient
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    DateOfBirth = model.DateOfBirth,
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

                var secretary = clinicDbContext.Secretaries.First();

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

                TempData["SuccessMessage"] = "Appointment created successfully";
                return RedirectToAction(nameof(CreateAppointment));
            }

            // ================== AJAX ==================
            [HttpGet]
            public IActionResult GetAvailableSlots(int doctorId, string date)
            {
                if (!DateTime.TryParse(date, out var day))
                    return Json(new List<string>());

                if (day.DayOfWeek == DayOfWeek.Friday)
                    return Json(new List<string>());

                var slots = new List<string>();
                var start = new TimeSpan(16, 0, 0);
                var end = new TimeSpan(20, 0, 0);

                for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
                {
                    bool booked = clinicDbContext.Appointments.Any(a =>
                        a.DoctorId == doctorId &&
                        a.AppointmentDate == day.Date &&
                        a.StartTime == t);

                    if (!booked)
                        slots.Add(t.ToString(@"hh\:mm"));
                }

                return Json(slots);


            }
        }
    }

}






       
























































































































































































//            [HttpGet]
//            public IActionResult CreateAppointment()
//            {
//                var model = new CreatePatient
//                {
//                    Doctors = clinicDbContext.Doctors.ToList()
//                };
//                return View(model);
//            }

//            // POST: Create Appointment
//            [HttpPost]
//            public IActionResult CreateAppointment(CreatePatient model)
//            {
//                if (!ModelState.IsValid)
//                {
//                    model.Doctors = clinicDbContext.Doctors.ToList();
//                    return View(model);
//                }

//                if (!TimeSpan.TryParse(model.SelectedTime, out var startTime))
//                {
//                    ModelState.AddModelError("SelectedTime", "Please select a valid time slot");
//                    model.Doctors = clinicDbContext.Doctors.ToList();
//                    return View(model);
//                }

//                var doctor = clinicDbContext.Doctors.FirstOrDefault(d => d.Id == model.DoctorId);
//                if (doctor == null)
//                {
//                    ModelState.AddModelError("DoctorId", "Invalid Doctor");
//                    model.Doctors = clinicDbContext.Doctors.ToList();
//                    return View(model);
//                }

//                bool booked = clinicDbContext.Appointments.Any(a =>
//                    a.DoctorId == model.DoctorId &&
//                    a.AppointmentDate == model.AppointmentDate.Date &&
//                    a.StartTime == startTime);

//                if (booked)
//                {
//                    ModelState.AddModelError("SelectedTime", "This time is already booked");
//                    model.Doctors = clinicDbContext.Doctors.ToList();
//                    return View(model);
//                }

//                var secretary = clinicDbContext.Secretaries.First();

//                var appointment = new Appointment
//                {
//                    DoctorId = doctor.Id,
//                    PatientId = 1,
//                    SecretaryId = secretary.Id,
//                    AppointmentDate = model.AppointmentDate.Date,
//                    StartTime = startTime,
//                    EndTime = startTime.Add(TimeSpan.FromMinutes(30))
//                };

//                clinicDbContext.Appointments.Add(appointment);
//                clinicDbContext.SaveChanges();

//                TempData["SuccessMessage"] = "Appointment created successfully";
//                return RedirectToAction(nameof(CreateAppointment));
//            }

//            // AJAX: Get Available Slots
//            [HttpGet]
//            public IActionResult GetAvailableSlots(int doctorId, string date)
//            {
//                if (!DateTime.TryParse(date, out var selectedDate))
//                {
//                    return Json(new { isWorking = false, slots = new List<string>(), message = "Invalid Date" });
//                }

//                var schedule = clinicDbContext.Schedules
//                    .FirstOrDefault(s => s.DoctorId == doctorId &&
//                                         s.DayOfWeek.Equals(selectedDate.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) &&
//                                         s.IsWorking);

//                if (schedule == null)
//                    return Json(new { isWorking = false, slots = new List<string>(), message = "Doctor is off on this day" });

//                var bookedSlots = clinicDbContext.Appointments
//                    .Where(a => a.DoctorId == doctorId && a.AppointmentDate == selectedDate.Date)
//                    .Select(a => a.StartTime)
//                    .ToList();

//                var slots = new List<string>();
//                for (var t = schedule.StartTime; t < schedule.EndTime; t = t.Add(TimeSpan.FromMinutes(30)))
//                {
//                    if (!bookedSlots.Contains(t))
//                        slots.Add(t.ToString(@"hh\:mm"));
//                }

//                return Json(new { isWorking = true, slots = slots, message = "" });
//            }
//        }
//    }
//}