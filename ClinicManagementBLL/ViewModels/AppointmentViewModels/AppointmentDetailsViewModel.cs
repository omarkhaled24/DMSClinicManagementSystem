using ClinicManagemnetDAL.Models;
using ClinicManagemnetDAL.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementBLL.ViewModels.AppointmentViewModels
{
    public class AppointmentDetailsViewModel
    {
        public string PatientName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;

        public string BuildingNumber { get; set; } = null!;
        public string FloorNumber { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;

        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

























        //public string PatientName { get; set; } = null!;
        //public string Email { get; set; } = null!;
        //public string Phone { get; set; } = null!;
        //public DateOnly DateOfBirth { get; set; }
        //public string Gender { get; set; } = null!;

        //public string BuildingNumber { get; set; } = null!;
        //public string FloorNumber { get; set; } = null!;
        //public string Street { get; set; } = null!;
        //public string City { get; set; } = null!;

        //public int DoctorId { get; set; }
        //public DateTime AppointmentDate { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        //public string SelectedTime { get; set; }
        //public List<Doctor> Doctors { get; set; } = new List<Doctor>();





        // Patient Info
        //[Required] public string Name { get; set; }
        //[Required][EmailAddress] public string Email { get; set; }
        //[Required] public string Phone { get; set; }
        //[Required] public DateOnly DateOfBirth { get; set; }
        //[Required] public Gender Gender { get; set; }

        //// Address
        //[Required] public string BuildingNumber { get; set; }
        //[Required] public string FloorNumber { get; set; }
        //[Required] public string Street { get; set; }
        //[Required] public string City { get; set; }

        //// Appointment
        //[Required] public int DoctorId { get; set; }
        //[Required] public DateTime AppointmentDate { get; set; }
        //[Required] public string SelectedTime { get; set; }

        //// Dropdown lists
        //public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();




        ////[Required]
        ////public int DoctorId { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
        //public DateTime AppointmentDate { get; set; }

        //[Required]
        //public string SelectedTime { get; set; }

        //public List<Doctor> Doctors { get; set; } = new List<Doctor>();

        //// Optional: For patient info
        //public string PatientName { get; set; }
        //public string PatientEmail { get; set; }
        //public string PatientPhone { get; set; }

    }
}
