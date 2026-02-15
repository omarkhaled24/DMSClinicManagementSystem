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
            // ===== Patient =====
            [Required]
            [StringLength(50, MinimumLength = 2)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can contain only letters and spaces")]
            public string Name { get; set; } = null!;

            [Required, EmailAddress]
            [Remote("IsEmailUnique", "Secretary", ErrorMessage = "Email already exists")]
            [StringLength(50, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 50 characters")]
            public string Email { get; set; } = null!;

            [Required]
            [Phone(ErrorMessage = "Invalid Phone Format")]
            [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be valid Egyptian number")]
            [Remote("IsPhoneUnique", "Secretary", ErrorMessage = "Phone already exists")]
            public string Phone { get; set; } = null!;

            [Required]
            public DateOnly? DateOfBirth { get; set; }

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
        public int SecretaryId;
        public List<Doctor> Doctors { get; set; } = new();

        }
}





