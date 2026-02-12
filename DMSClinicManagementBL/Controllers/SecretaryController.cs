
using ClinicManagementBLL.ViewModels.PatientViewModel;
using ClinicManagemnetDAL.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagemnetDAL.Models; // هنا موجود Appointment, Patient, Doctor
using ClinicManagemnetDAL.Data;   // هنا موجود DbContext
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ClinicManagemnetDAL.Models.Enums;
using ClinicManagementBLL.ViewModels.AppointmentViewModels;

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







//            // GET: Create Appointment
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
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);

//                // Model validation
//                if (!ModelState.IsValid)
//                    return View(model);

//                // Unique checks
//                if (clinicDbContext.Patients.Any(p => p.Email == model.Email))
//                    ModelState.AddModelError("Email", "Email already exists");

//                if (clinicDbContext.Patients.Any(p => p.PhoneNumber == model.Phone))
//                    ModelState.AddModelError("Phone", "Phone already exists");

//                if (model.SelectedTime == null)
//                    ModelState.AddModelError("SelectedTime", "Please select a valid time slot");

//                if (model.AppointmentDate.DayOfWeek == DayOfWeek.Friday)
//                    ModelState.AddModelError("AppointmentDate", "Doctor is off on Friday");

//                if (!ModelState.IsValid)
//                    return View(model);

//                // Address
//                var address = clinicDbContext.Addresses.FirstOrDefault(a =>
//                    a.BuildingNumber == model.BuildingNumber &&
//                    a.Street == model.Street &&
//                    a.City == model.City &&
//                    a.FloorNumber == model.FloorNumber);

//                if (address == null)
//                {
//                    address = new Address
//                    {
//                        BuildingNumber = model.BuildingNumber,
//                        Street = model.Street,
//                        City = model.City,
//                        FloorNumber = model.FloorNumber
//                    };
//                    clinicDbContext.Addresses.Add(address);
//                    clinicDbContext.SaveChanges();
//                }

//                // Patient
//                var patient = clinicDbContext.Patients.FirstOrDefault(p => p.Email == model.Email);
//                if (patient == null)
//                {
//                    patient = new Patient
//                    {
//                        Name = model.Name,
//                        DateOfBirth = model.DateOfBirth,
//                        PhoneNumber = model.Phone,
//                        Email = model.Email,
//                        Gender = model.Gender,
//                        AddressId = address.Id
//                    };
//                    clinicDbContext.Patients.Add(patient);
//                    clinicDbContext.SaveChanges();
//                }

//                // Doctor
//                var doctor = clinicDbContext.Doctors.FirstOrDefault(d => d.Id == model.DoctorId);
//                if (doctor == null)
//                {
//                    ModelState.AddModelError("", "Doctor not found.");
//                    return View(model);
//                }

//                // Secretary (أول واحد موجود)
//                var secretary = clinicDbContext.Secretaries.FirstOrDefault();
//                if (secretary == null)
//                {
//                    ModelState.AddModelError("", "No secretary found in system.");
//                    return View(model);
//                }

//                // Appointment
//                var appointment = new Appointment
//                {
//                    DoctorId = doctor.Id,
//                    PatientId = patient.Id,
//                    SecretaryId = secretary.Id,
//                    AppointmentDate = model.AppointmentDate,
//                    StartTime = model.SelectedTime.Value,
//                    EndTime = model.SelectedTime.Value.Add(TimeSpan.FromMinutes(30))
//                };

//                clinicDbContext.Appointments.Add(appointment);
//                clinicDbContext.SaveChanges();

//                TempData["SuccessMessage"] = "Appointment created successfully!";
//                return RedirectToAction("CreateAppointment");
//            }

//            // AJAX: Get available time slots
//            [HttpGet]
//            public IActionResult GetAvailableSlots(int doctorId, string date)
//            {
//                if (!DateTime.TryParse(date, out var appointmentDate))
//                    return Json(new List<string>());

//                var slots = GetAvailableSlots(doctorId, appointmentDate);
//                return Json(slots.Select(s => s.ToString(@"hh\:mm")));
//            }

//            // Helper: Generate available slots
//            private List<TimeSpan> GetAvailableSlots(int doctorId, DateTime date)
//            {
//                var slots = new List<TimeSpan>();

//                if (date.DayOfWeek == DayOfWeek.Friday) return slots; // Doctor off

//                var start = new TimeSpan(16, 0, 0);
//                var end = new TimeSpan(20, 0, 0);
//                for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
//                {
//                    bool booked = clinicDbContext.Appointments
//                        .Any(a => a.DoctorId == doctorId && a.AppointmentDate == date && a.StartTime == t);
//                    if (!booked) slots.Add(t);
//                }
//                return slots;
//            }

//            // Remote Validation
//            [AcceptVerbs("GET", "POST")]
//            public IActionResult IsEmailUnique(string email)
//            {
//                return Json(!clinicDbContext.Patients.Any(p => p.Email == email));
//            }

//            [AcceptVerbs("GET", "POST")]
//            public IActionResult IsPhoneUnique(string phone)
//            {
//                return Json(!clinicDbContext.Patients.Any(p => p.PhoneNumber == phone));
//            }
//        }
//    }
//}











//            public IActionResult IsEmailUnique(string email)
//            {
//                bool exists = clinicDbContext.Patients.Any(p => p.Email == email);
//                return Json(!exists);
//            }

//            public IActionResult IsPhoneUnique(string phone)
//            {
//                bool exists = clinicDbContext.Patients.Any(p => p.PhoneNumber == phone);
//                return Json(!exists);
//            }

//            // GET: Create Appointment
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
//                    model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
//                    return View(model);
//                }

//                // Unique Checks
//                if (clinicDbContext.Patients.Any(p => p.Email == model.Email))
//                {
//                    ModelState.AddModelError("Email", "Email already exists");
//                }
//                if (clinicDbContext.Patients.Any(p => p.PhoneNumber == model.Phone))
//                {
//                    ModelState.AddModelError("Phone", "Phone already exists");
//                }
//                if (!ModelState.IsValid)
//                {
//                    model.Doctors = clinicDbContext.Doctors.ToList();
//                    model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
//                    return View(model);
//                }

//                // Address
//                var address = clinicDbContext.Addresses.FirstOrDefault(a =>
//                    a.BuildingNumber == model.BuildingNumber &&
//                    a.Street == model.Street &&
//                    a.City == model.City &&
//                    a.FloorNumber == model.FloorNumber);

//                if (address == null)
//                {
//                    address = new Address
//                    {
//                        BuildingNumber = model.BuildingNumber,
//                        Street = model.Street,
//                        City = model.City,
//                        FloorNumber = model.FloorNumber
//                    };
//                    clinicDbContext.Addresses.Add(address);
//                    clinicDbContext.SaveChanges();
//                }

//                // Patient
//                var patient = new Patient
//                {
//                    Name = model.Name,
//                    DateOfBirth = model.DateOfBirth,
//                    PhoneNumber = model.Phone,
//                    Email = model.Email,
//                    Gender = model.Gender,
//                    AddressId = address.Id
//                };
//                clinicDbContext.Patients.Add(patient);
//                clinicDbContext.SaveChanges();

//                // Doctor
//                var doctor = clinicDbContext.Doctors.Include(d => d.schedules)
//                    .FirstOrDefault(d => d.Id == model.DoctorId);

//                if (doctor == null || model.AppointmentDate.DayOfWeek == DayOfWeek.Friday || model.SelectedTime == null)
//                {
//                    ModelState.AddModelError("", "Invalid Doctor/Date/Time selection");
//                    model.Doctors = clinicDbContext.Doctors.ToList();
//                    model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
//                    return View(model);
//                }

//                // Secretary (أول واحد موجود)
//                var secretary = clinicDbContext.Secretaries.AsNoTracking().FirstOrDefault();
//                if (secretary == null)
//                {
//                    ModelState.AddModelError("", "No secretary found in system.");
//                    return View(model);
//                }

//                // Appointment
//                var appointment = new Appointment
//                {
//                    DoctorId = doctor.Id,
//                    PatientId = patient.Id,
//                    SecretaryId = secretary.Id,
//                    AppointmentDate = model.AppointmentDate,
//                    StartTime = model.SelectedTime.Value,
//                    EndTime = model.SelectedTime.Value.Add(TimeSpan.FromMinutes(30))
//                };

//                clinicDbContext.Appointments.Add(appointment);
//                clinicDbContext.SaveChanges();

//                TempData["Success"] = "Appointment created successfully!";
//                return RedirectToAction("CreateAppointment");

//            }

//            // AJAX: Get available time slots
//            [HttpGet]
//            public IActionResult GetAvailableSlots(int doctorId, string date)
//            {
//                if (!DateTime.TryParse(date, out var appointmentDate))
//                    return Json(new List<string>());

//                var slots = GetAvailableSlots(doctorId, appointmentDate);
//                return Json(slots.Select(s => s.ToString(@"hh\:mm")));
//            }

//            // Helper method to get slots
//            private List<TimeSpan> GetAvailableSlots(int doctorId, DateTime date)
//            {
//                var slots = new List<TimeSpan>();

//                if (date.DayOfWeek == DayOfWeek.Friday) return slots; // Doctor off

//                // Doctor working 16:00 - 20:00
//                var start = new TimeSpan(16, 0, 0);
//                var end = new TimeSpan(20, 0, 0);
//                for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
//                {
//                    // Check if already booked
//                    bool booked = clinicDbContext.Appointments
//                        .Any(a => a.DoctorId == doctorId && a.AppointmentDate == date && a.StartTime == t);
//                    if (!booked) slots.Add(t);
//                }

//                return slots;

//            }
//        }
//    }
//}




























