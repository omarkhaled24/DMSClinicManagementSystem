//using ClinicManagementBLL.Services.Classes;
//using ClinicManagementBLL.Services.Interface;
//using ClinicManagementBLL.ViewModels.PatientViewModel;
//using ClinicManagemnetDAL.Data.Contexts;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

//namespace DMSClinicManagementBL.Controllers
//{
//    public class SecretaryController : Controller
//    {
//        private readonly ClinicDbContext clinicDbContext;
//        private readonly IAppointmentService appointmentService;

//        public SecretaryController(ClinicDbContext clinicDbContext, IAppointmentService appointmentService)
//        {
//            this.clinicDbContext = clinicDbContext;
//            this.appointmentService = appointmentService;
//        }
//        [HttpGet]
//        public IActionResult CreateAppointment()
//        {
//            var model = new CreatePatient
//            {
//                Doctors = clinicDbContext.Doctors.ToList()
//            };

//            return View(model);
//        }
//    }
//}



//using ClinicManagementBLL.Services.Classes;
//using ClinicManagementBLL.Services.Interface;
//using ClinicManagementBLL.ViewModels.PatientViewModel;
//using ClinicManagemnetDAL.Data.Contexts;
//using ClinicManagemnetDAL.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace DMSClinicManagementBL.Controllers
//{
//    public class SecretaryController : Controller
//    {
//        private readonly ClinicDbContext clinicDbContext;
//        private readonly IAppointmentService appointmentService;

//        public SecretaryController(ClinicDbContext clinicDbContext, IAppointmentService appointmentService)
//        {
//            this.clinicDbContext = clinicDbContext;
//            this.appointmentService = appointmentService;
//        }

//        // GET: Create Appointment
//        [HttpGet]
//        public IActionResult CreateAppointment()
//        {
//            var model = new CreatePatient
//            {
//                Doctors = clinicDbContext.Doctors.ToList(),
//                AvailableSlots = new List<TimeSpan>()
//            };
//            return View(model);
//        }

//        // POST: Create Appointment
//        [HttpPost]
//        public IActionResult CreateAppointment(CreatePatient model)
//        {
//            if (model.DoctorId == 0 || model.AppointmentDate == default)
//            {
//                ModelState.AddModelError("", "⚠️ Please select a doctor and a valid date.");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                model.AvailableSlots = new List<TimeSpan>();
//                return View(model);
//            }

//            int dayOfWeek = (int)model.AppointmentDate.DayOfWeek;
//            dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;

//            var schedule = clinicDbContext.Schedules
//                .FirstOrDefault(s => s.DoctorId == model.DoctorId && (int)s.DayOfWeek == dayOfWeek);

//            if (schedule == null)
//            {
//                ModelState.AddModelError("", "⚠️ Doctor is on leave on this day!");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                model.AvailableSlots = new List<TimeSpan>();
//                return View(model);
//            }

//            // تحقق أو إنشاء Address
//            Address address = clinicDbContext.Addresses
//      .FirstOrDefault(a => a.BuildingNumber == model.BuildingNumber &&
//                           a.Street == model.Street &&
//                           a.City == model.City &&
//                           a.FloorNumber == model.FloorNumber);

//            if (address == null)
//            {
//                address = new Address
//                {
//                    BuildingNumber = model.BuildingNumber,
//                    Street = model.Street,
//                    City = model.City,
//                    FloorNumber = model.FloorNumber // <--- مهم
//                };
//                clinicDbContext.Addresses.Add(address);
//                clinicDbContext.SaveChanges();
//            }


//            // تحقق أو إنشاء المريض مرتبط بالعنوان
//            var patient = clinicDbContext.Patients
//                .FirstOrDefault(p => p.Name == model.Name && p.DateOfBirth == model.DateOfBirth);

//            if (patient == null)
//            {
//                patient = new Patient
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
//            }

//            if (!model.SelectedTime.HasValue)
//            {
//                ModelState.AddModelError("", "⚠️ Please select a valid time slot.");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
//                return View(model);
//            }

//            // إنشاء الموعد
//            var appointment = new Appointment
//            {
//                PatientId = patient.Id, // تأكد اسم المفتاح الصحيح في Patient
//                DoctorId = model.DoctorId,
//                AppointmentDate = model.AppointmentDate,
//                StartTime = model.SelectedTime.Value,
//                EndTime = model.SelectedTime.Value.Add(TimeSpan.FromMinutes(30)),
//                SecretaryId =model.SecretaryId,

//            };

//            clinicDbContext.Appointments.Add(appointment);
//            clinicDbContext.SaveChanges();

//            return RedirectToAction("AppointmentsList");
//        }

//        // GET: Available Times for AJAX
//        public JsonResult GetAvailableTimes(int doctorId, DateTime date)
//        {
//            var slots = GetAvailableSlots(doctorId, date);

//            if (!slots.Any())
//                return Json(new { available = false, times = new List<string>() });

//            return Json(new { available = true, times = slots.Select(t => t.ToString(@"hh\:mm")).ToList() });
//        }

//        // GET: List of Appointments
//        public IActionResult AppointmentsList()
//        {
//            var appointments = clinicDbContext.Appointments
//                .Include(a => a.Patient)
//                .Include(a => a.Doctor)
//                .OrderBy(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .ToList();

//            return View(appointments);
//        }

//        // دالة مساعدة لحساب الأوقات المتاحة
//        private List<TimeSpan> GetAvailableSlots(int doctorId, DateTime date)
//        {
//            int dayOfWeek = (int)date.DayOfWeek;
//            dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;

//            var schedule = clinicDbContext.Schedules
//                .FirstOrDefault(s => s.DoctorId == doctorId && (int)s.DayOfWeek == dayOfWeek);

//            if (schedule == null) return new List<TimeSpan>();

//            var bookedTimes = clinicDbContext.Appointments
//                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
//                .Select(a => a.StartTime)
//                .ToList();

//            var times = new List<TimeSpan>();
//            var start = schedule.StartTime;
//            while (start + TimeSpan.FromMinutes(30) <= schedule.EndTime)
//            {
//                if (!bookedTimes.Contains(start))
//                    times.Add(start);
//                start = start.Add(TimeSpan.FromMinutes(30));
//            }

//            return times;
//        }
//    }
//}







//using ClinicManagementBLL.Services.Interface;
//using ClinicManagementBLL.ViewModels.PatientViewModel;
//using ClinicManagemnetDAL.Data.Contexts;
//using ClinicManagemnetDAL.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;

//namespace DMSClinicManagementBL.Controllers
//{
//    public class SecretaryController : Controller
//    {
//        private readonly ClinicDbContext clinicDbContext;
//        private readonly IAppointmentService appointmentService;

//        public SecretaryController(ClinicDbContext clinicDbContext, IAppointmentService appointmentService)
//        {
//            this.clinicDbContext = clinicDbContext;
//            this.appointmentService = appointmentService;
//        }

//        [HttpGet]
//        public IActionResult CreateAppointment()
//        {
//            var model = new CreatePatient
//            {
//                Doctors = clinicDbContext.Doctors.ToList()
//            };
//            return View(model);
//        }

//        [HttpPost]
//        public IActionResult CreateAppointment(CreatePatient model)
//        {
//            if (!ModelState.IsValid)
//            {
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                return View(model);
//            }

//            // تحقق أو أنشئ Address
//            Address address = clinicDbContext.Addresses
//                .FirstOrDefault(a => a.BuildingNumber == model.BuildingNumber &&
//                                     a.Street == model.Street &&
//                                     a.City == model.City &&
//                                     a.FloorNumber == model.FloorNumber);

//            if (address == null)
//            {
//                address = new Address
//                {
//                    BuildingNumber = model.BuildingNumber,
//                    Street = model.Street,
//                    City = model.City,
//                    FloorNumber = model.FloorNumber
//                };
//                clinicDbContext.Addresses.Add(address);
//                clinicDbContext.SaveChanges();
//            }

//            // تحقق أو أنشئ Patient (تجنب Duplicate Email)
//            var patient = clinicDbContext.Patients.FirstOrDefault(p => p.Email == model.Email);

//            if (patient == null)
//            {
//                patient = new Patient
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
//            }

//            // تحقق من Doctor Schedule
//            var doctor = clinicDbContext.Doctors
//                .Include(d => d.schedules)
//                .FirstOrDefault(d => d.Id == model.DoctorId);

//            if (doctor == null)
//            {
//                ModelState.AddModelError("", "Doctor not found.");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                return View(model);
//            }

//            // مثال: ممنوع يوم الجمعة
//            if (model.AppointmentDate.DayOfWeek == DayOfWeek.Friday)
//            {
//                ModelState.AddModelError("", "Doctor is on leave on this day!");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                return View(model);
//            }

//            // TimeSlots
//            if (model.SelectedTime == null)
//            {
//                ModelState.AddModelError("", "Please select a valid time slot.");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                return View(model);
//            }

//            // حدد SecretaryId (ثابت للتجربة: ID=1)
//            int secretaryId = 1;

//            // إنشاء Appointment
//            var appointment = new Appointment
//            {
//                PatientId = patient.Id,
//                DoctorId = doctor.Id,
//                AppointmentDate = model.AppointmentDate,
//                StartTime = model.SelectedTime.Value,
//                EndTime = model.SelectedTime.Value.Add(TimeSpan.FromMinutes(30)),
//                SecretaryId = secretaryId
//            };

//            clinicDbContext.Appointments.Add(appointment);
//            clinicDbContext.SaveChanges();

//            TempData["Success"] = "Appointment created successfully!";
//            return RedirectToAction("CreateAppointment");
//        }
//    }
//}

using ClinicManagementBLL.Services.Interface;
using ClinicManagementBLL.ViewModels.PatientViewModel;
using ClinicManagemnetDAL.Data.Contexts;
using ClinicManagemnetDAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DMSClinicManagementBL.Controllers
{
    public class SecretaryController : Controller
    {
        private readonly ClinicDbContext clinicDbContext;
        private readonly IAppointmentService appointmentService;

        public SecretaryController(ClinicDbContext clinicDbContext, IAppointmentService appointmentService)
        {
            this.clinicDbContext = clinicDbContext;
            this.appointmentService = appointmentService;
        }

        [HttpGet]
        public IActionResult CreateAppointment()
        {
            var model = new CreatePatient
            {
                Doctors = clinicDbContext.Doctors.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateAppointment(CreatePatient model)
        {
            if (!ModelState.IsValid)
            {
                model.Doctors = clinicDbContext.Doctors.ToList();
                model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
                return View(model);
            }

            // Address
            Address address = clinicDbContext.Addresses.FirstOrDefault(a =>
                a.BuildingNumber == model.BuildingNumber &&
                a.Street == model.Street &&
                a.City == model.City &&
                a.FloorNumber == model.FloorNumber);

            if (address == null)
            {
                address = new Address
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City,
                    FloorNumber = model.FloorNumber
                };
                clinicDbContext.Addresses.Add(address);
                clinicDbContext.SaveChanges();
            }

            // Patient
            var patient = clinicDbContext.Patients.FirstOrDefault(p => p.Email == model.Email);
            if (patient == null)
            {
                patient = new Patient
                {
                    Name = model.Name,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.Phone,
                    Email = model.Email,
                    Gender = model.Gender,
                    AddressId = address.Id
                };
                clinicDbContext.Patients.Add(patient);
                clinicDbContext.SaveChanges();
            }

            // Doctor
            var doctor = clinicDbContext.Doctors.Include(d => d.schedules)
                .FirstOrDefault(d => d.Id == model.DoctorId);

            if (doctor == null)
            {
                ModelState.AddModelError("", "Doctor not found.");
                model.Doctors = clinicDbContext.Doctors.ToList();
                model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
                return View(model);
            }

            // Doctor Leave
            if (model.AppointmentDate.DayOfWeek == DayOfWeek.Friday)
            {
                ModelState.AddModelError("", "Doctor is on leave on this day!");
                model.Doctors = clinicDbContext.Doctors.ToList();
                model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
                return View(model);
            }

            // Selected Time
            if (model.SelectedTime == null)
            {
                ModelState.AddModelError("", "Please select a valid time slot.");
                model.Doctors = clinicDbContext.Doctors.ToList();
                model.AvailableSlots = GetAvailableSlots(model.DoctorId, model.AppointmentDate);
                return View(model);
            }

            // Secretary ID ثابت للتجربة
            int secretaryId = 1;

            // Appointment
            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentDate = model.AppointmentDate,
                StartTime = model.SelectedTime.Value,
                EndTime = model.SelectedTime.Value.Add(TimeSpan.FromMinutes(30)),
                SecretaryId = secretaryId
            };

            clinicDbContext.Appointments.Add(appointment);
            clinicDbContext.SaveChanges();

            TempData["Success"] = "Appointment created successfully!";
            return RedirectToAction("CreateAppointment");
        }

        // ===================== Helper Method =====================
        private List<TimeSpan> GetAvailableSlots(int doctorId, DateTime date)
        {
            var slots = new List<TimeSpan>();

            // يوم الجمعة Doctor off
            if (date.DayOfWeek == DayOfWeek.Friday)
                return slots;

            // افتراض: Doctor working from 16:00 to 20:00
            var start = new TimeSpan(16, 0, 0);
            var end = new TimeSpan(20, 0, 0);
            var allSlots = new List<TimeSpan>();

            for (var time = start; time < end; time = time.Add(TimeSpan.FromMinutes(30)))
                allSlots.Add(time);

            // اخذ مواعيد موجودة مسبقًا
            var bookedSlots = clinicDbContext.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
                .Select(a => a.StartTime)
                .ToList();

            slots = allSlots.Where(s => !bookedSlots.Contains(s)).ToList();
            return slots;
        }
    }
}



//using ClinicManagementBLL.Services.Classes;
//using ClinicManagementBLL.Services.Interface;
//using ClinicManagementBLL.ViewModels.PatientViewModel;
//using ClinicManagemnetDAL.Data.Contexts;
//using ClinicManagemnetDAL.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;

//namespace DMSClinicManagementBL.Controllers
//{
//    public class SecretaryController : Controller
//    {
//        private readonly ClinicDbContext clinicDbContext;
//        private readonly IAppointmentService appointmentService;

//        public SecretaryController(ClinicDbContext clinicDbContext, IAppointmentService appointmentService)
//        {
//            this.clinicDbContext = clinicDbContext;
//            this.appointmentService = appointmentService;
//        }

//        // GET: Create Appointment
//        [HttpGet]
//        public IActionResult CreateAppointment()
//        {
//            var model = new CreatePatient
//            {
//                Doctors = clinicDbContext.Doctors.ToList()
//            };
//            return View(model);
//        }

//        // POST: Create Appointment
//        [HttpPost]
//        public IActionResult CreateAppointment(CreatePatient model, int doctorId, DateTime date, TimeSpan? time)
//        {
//            // تحقق من جدول الدكتور
//            var schedule = clinicDbContext.Schedules
//                .FirstOrDefault(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek);

//            if (schedule == null)
//            {
//                ModelState.AddModelError("", "⚠️ Doctor is on leave on this day!");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                return View(model);
//            }

//            // تحقق أو إنشاء المريض
//            var patient = clinicDbContext.Patients
//                .FirstOrDefault(p => p.Name == model.Name && p.DateOfBirth == model.DateOfBirth);

//            if (patient == null)
//            {
//                patient = new Patient
//                {
//                    Name = model.Name,
//                    DateOfBirth = model.DateOfBirth,
//                   PhoneNumber = model.Phone,
//                    Email = model.Email
//                };
//                clinicDbContext.Patients.Add(patient);
//                clinicDbContext.SaveChanges();
//            }

//            if (!time.HasValue)
//            {
//                ModelState.AddModelError("", "⚠️ Please select a valid time slot.");
//                model.Doctors = clinicDbContext.Doctors.ToList();
//                return View(model);
//            }

//            // إنشاء الموعد
//            var appointment = new Appointment
//            {
//                PatientId = patient.Id,
//                DoctorId = doctorId,
//                AppointmentDate = date,
//                StartTime = time.Value,
//                EndTime = time.Value.Add(TimeSpan.FromMinutes(30))
//            };
//            clinicDbContext.Appointments.Add(appointment);
//            clinicDbContext.SaveChanges();

//            return RedirectToAction("AppointmentsList");
//        }

//        // GET: Available Times for AJAX
//        public JsonResult GetAvailableTimes(int doctorId, DateTime date)
//        {
//            var schedule = clinicDbContext.Schedules
//                .FirstOrDefault(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek);

//            if (schedule == null)
//                return Json(new { available = false, times = new List<string>() });

//            var bookedTimes = clinicDbContext.Appointments
//                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
//                .Select(a => a.StartTime)
//                .ToList();

//            var times = new List<string>();
//            var start = schedule.StartTime;
//            while (start + TimeSpan.FromMinutes(30) <= schedule.EndTime)
//            {
//                if (!bookedTimes.Contains(start))
//                    times.Add(start.ToString(@"hh\:mm"));
//                start = start.Add(TimeSpan.FromMinutes(30));
//            }

//            return Json(new { available = true, times });
//        }

//        // GET: List of Appointments
//        public IActionResult AppointmentsList()
//        {
//            var appointments = clinicDbContext.Appointments
//                .Include(a => a.Patient)
//                .Include(a => a.Doctor)
//                .ToList();

//            return View(appointments);
//        }
//    }
//}
