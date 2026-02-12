using ClinicManagemnetDAL.Models;
using ClinicManagemnetDAL.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementBLL.ViewModels.PatientViewModel
{
        
    public class CreatePatient
    {

        [Required]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        // ===== Address =====
        [Required]
        public string BuildingNumber { get; set; } = null!;

        [Required]
        public string FloorNumber { get; set; } = null!;

        [Required]
        public string Street { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        // ===== Appointment =====
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string SelectedTime { get; set; } = null!;

        // ===== Helpers =====








        //[Required(ErrorMessage = "Name Is Required")]
        //[StringLength(50, MinimumLength = 2)]
        //[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can contain only letters and spaces")]
        //public string Name { get; set; } = null!;

        //[Required(ErrorMessage = "Email Is Required")]
        //[EmailAddress(ErrorMessage = "Invalid Email Format")]
        //[Remote("IsEmailUnique", "Secretary", ErrorMessage = "Email already exists")]
        //[StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        //public string Email { get; set; } = null!;

        //[Required(ErrorMessage = "Phone Is Required")]
        //[Phone(ErrorMessage = "Invalid Phone Format")]
        //[RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be valid Egyptian number")]
        //[Remote("IsPhoneUnique", "Secretary", ErrorMessage = "Phone already exists")]
        //public string Phone { get; set; } = null!;

        //[Required(ErrorMessage = "Date Of Birth Is Required")]
        //[DataType(DataType.Date)]
        //public DateOnly DateOfBirth { get; set; }

        //[Required(ErrorMessage = "Gender Is Required")]
        //public Gender Gender { get; set; }

        //// Address
        //[Required]
        //[Range(1, 200)]
        //public string BuildingNumber { get; set; } = null!;
        //public string FloorNumber { get; set; } = null!;
        //[Required]
        //[StringLength(30, MinimumLength = 2)]
        //public string Street { get; set; } = null!;
        //[Required]
        //[StringLength(20, MinimumLength = 2)]
        //[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City can contain only letters and spaces")]
        //public string City { get; set; } = null!;

        //// Appointment
        //public int DoctorId { get; set; }
        //public DateTime AppointmentDate { get; set; }
        //public string SelectedTime { get; set; } = string.Empty;

        //// Dropdowns
        public List<Doctor> Doctors { get; set; } = new();
        public List<TimeSpan> AvailableSlots { get; set; } = new();

    }
}





