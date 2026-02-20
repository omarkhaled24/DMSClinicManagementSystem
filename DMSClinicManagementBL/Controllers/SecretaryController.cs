using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManagemnetDAL.Data.Contexts;       
using ClinicManagemnetDAL.Models;              
using ClinicManagemnetDAL.Models.Enums;       
using ClinicManagementBLL.ViewModels.PatientViewModel; 



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

            // POST 
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

                if (model.DateOfBirth > DateOnly.FromDateTime(DateTime.Today))
                {
                    ModelState.AddModelError("DateOfBirth", "Birth date cannot be in the future.");
                }

                if (model.AppointmentDate.Value.Date < DateTime.Today)
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
                    a.AppointmentDate == model.AppointmentDate.Value.Date &&
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
                    AppointmentDate = model.AppointmentDate.Value.Date,
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

                // Prevent past dates
                if (day.Date < DateTime.Today)
                    return Json(new { isOff = true, message = "Cannot select a past date" });

                var dayOfWeek = day.DayOfWeek; // DayOfWeek enum

                var schedule = clinicDbContext.Schedules
                    .FirstOrDefault(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek && s.IsWorking);

                if (schedule == null)
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
            public IActionResult PatientsWithAppointments(string searchName, string dateFilter, string doctorFilter, int page = 1)
            {
                int pageSize = 5;

                var query = clinicDbContext.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .AsQueryable();

                // Filter by patient name
                if (!string.IsNullOrEmpty(searchName))
                    query = query.Where(a => a.Patient.Name.Contains(searchName));

                // Filter by appointment date
                if (DateTime.TryParse(dateFilter, out var date))
                    query = query.Where(a => a.AppointmentDate.Date == date.Date);

                // Filter by doctor
                if (!string.IsNullOrEmpty(doctorFilter) && int.TryParse(doctorFilter, out var docId))
                    query = query.Where(a => a.DoctorId == docId);

                // Pagination
                int totalItems = query.Count();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var appointments = query
                    .OrderBy(a => a.AppointmentDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                
                var model = appointments.Select(a => new PatientAppointmentList
                {
                    PatientId = a.Patient.Id,
                    PatientName = a.Patient.Name,
                    DoctorName = a.Doctor.Name,
                    Phone = a.Patient.PhoneNumber,
                    Email = a.Patient.Email,
                    AppointmentDate = a.AppointmentDate
                }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.SearchName = searchName;
                ViewBag.DateFilter = dateFilter;
                ViewBag.DoctorFilter = doctorFilter;

                // Doctor dropdown list
                ViewBag.Doctors = clinicDbContext.Doctors.ToList();

                return View(model);
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
                    Id = appointment.Patient.Id,  
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

                // Prevent Chooseing Old Date
                if (model.AppointmentDate.Date < DateTime.Today)
                {
                    ModelState.AddModelError("AppointmentDate", "You cannot select a past date.");
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }

                // Getting the patient's current data
                var appointment = clinicDbContext.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Patient.Address)
                    .FirstOrDefault(a => a.PatientId == model.Id);

                if (appointment == null)
                    return RedirectToAction(nameof(PatientsWithAppointments));

                // Check Email & Phone
                if (clinicDbContext.Patients.Any(p => p.Email == model.Email && p.Id != model.Id))
                    ModelState.AddModelError("Email", "Email already exists");

                if (clinicDbContext.Patients.Any(p => p.PhoneNumber == model.Phone && p.Id != model.Id))
                    ModelState.AddModelError("Phone", "Phone already exists");

                if (!ModelState.IsValid)
                {
                    model.Doctors = clinicDbContext.Doctors.ToList();
                    return View(model);
                }

                // Refresh Data
                appointment.Patient.Name = model.Name;
                appointment.Patient.Email = model.Email;
                appointment.Patient.PhoneNumber = model.Phone;

                if (appointment.Patient.Address == null)
                    appointment.Patient.Address = new Address();

                appointment.Patient.Address.BuildingNumber = model.BuildingNumber;
                appointment.Patient.Address.FloorNumber = model.FloorNumber;
                appointment.Patient.Address.Street = model.Street;
                appointment.Patient.Address.City = model.City;

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
