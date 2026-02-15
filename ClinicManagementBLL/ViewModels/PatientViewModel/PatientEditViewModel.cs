using ClinicManagemnetDAL.Models.Enums;
using ClinicManagemnetDAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementBLL.ViewModels.PatientViewModel
{
    public class PatientEditViewModel
    {
        // Patient
        public int Id { get; set; }
        
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can contain only letters and spaces")]
        public string Name { get; set; } = null!;

        [EmailAddress]
        //[Remote("IsEmailUnique", "Secretary", ErrorMessage = "Email already exists")]
        //[StringLength(50, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 50 characters")]
        public string Email { get; set; } = null!;

        //[Required]
        //[Phone(ErrorMessage = "Invalid Phone Format")]
        //[RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be valid Egyptian number")]
        //[Remote("IsPhoneUnique", "Secretary", ErrorMessage = "Phone already exists")]
        public string Phone { get; set; } = null!;


        // Address
        [Required]
        public string BuildingNumber { get; set; } = null!;
        [Required]
        public string FloorNumber { get; set; } = null!;
        [Required]
        public string Street { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;

        // Doctor 
        public int DoctorId { get; set; } 
        public DateTime AppointmentDate { get; set; }
        public string SelectedTime { get; set; } = null!;

        public List<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
