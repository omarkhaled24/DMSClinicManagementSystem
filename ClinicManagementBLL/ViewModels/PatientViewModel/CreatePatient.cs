using ClinicManagemnetDAL.Models;
using ClinicManagemnetDAL.Models.Enums;
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
        //    // Patient Data 
        //    [Required(ErrorMessage = "Name Is Required")]
        //    [StringLength(maximumLength: 50, MinimumLength = 2)]
        //    [RegularExpression(pattern: @"^[a-zA-Z\s ]+$")]
        //    public string Name { get; set; } = null!;

        //    [Required(ErrorMessage = "Email Is Required")]
        //    [EmailAddress(ErrorMessage = "Invalid Email Format")]
        //    [DataType(dataType: DataType.EmailAddress)]
        //    [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Email Must Be 5 and 100 char")]
        //    public string Email { get; set; } = null!;

        //    [Required(ErrorMessage = "Phone Is Required")]
        //    [Phone(ErrorMessage = "Invalid Phone Format")]
        //    [RegularExpression(pattern: @"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone Number Must Be Valid Egyptian PhoneNumber")]
        //    [DataType(dataType: DataType.PhoneNumber)]
        //    public string Phone { get; set; } = null!;

        //    [Required(ErrorMessage = "Date Of Birth Is Required")]
        //    [DataType(dataType: DataType.Date)]
        //    public DateOnly DateOfBirth { get; set; }

        //    [Required(ErrorMessage = "Gender Is Required")]

        //    public Gender Gender { get; set; }

        //    [Required(ErrorMessage = "Building Number Is Required")]
        //    [Range(minimum: 1, maximum: 200, ErrorMessage = "Building Number Must Be Between 1 and 200")]
        //    public string BuildingNumber { get; set; } = null!;
        //    public string FloorNumber { get; set; } = null!;

        //    [Required(ErrorMessage = "Street Is Required")]
        //    [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "Street Must Be Between 2 And 30 chars")]
        //    public string Street { get; set; } = null!;

        //    [Required(ErrorMessage = "City Is Required")]
        //    [StringLength(maximumLength: 20, MinimumLength = 2, ErrorMessage = "City Must Be Between 2 And 20 chars")]
        //    [RegularExpression(pattern: @"^[a-zA-Z\s ]+$", ErrorMessage = "City Can Contain Only Letters And Spaces")]
        //    public string City { get; set; } = null!;
        //    public int SecretaryId { get; set; }

        //    // Appointment Data
        //    public int DoctorId { get; set; }
        //    public DateTime AppointmentDate { get; set; }
        //    public TimeSpan? SelectedTime { get; set; }

        //    // For Dropdowns
        //    public List<Doctor> Doctors { get; set; } = new();
        //    public List<TimeSpan> AvailableSlots { get; set; } = new();
    //    [Required] public string Name { get; set; }
    //[Required, DataType(DataType.Date)] public DateOnly DateOfBirth { get; set; }
    //[Required, EmailAddress] public string Email { get; set; }
    //[Required] public string Phone { get; set; }
    //[Required] public Gender Gender { get; set; }

    //// Address Info
    //[Required] public string BuildingNumber { get; set; }
    //[Required] public string Street { get; set; }
    //[Required] public string City { get; set; }
    //[Required] public string FloorNumber { get; set; }

    //// Appointment Info
    //[Required] public int DoctorId { get; set; }
    //[Required, DataType(DataType.Date)] public DateTime AppointmentDate { get; set; }
    //public TimeSpan? SelectedTime { get; set; }

    //// UI Helpers
    //public List<ClinicManagemnetDAL.Models.Doctor> Doctors { get; set; } = new();
    //public List<TimeSpan> AvailableSlots { get; set; } = new();

            // Patient Info
            [Required] public string Name { get; set; }
            [Required, DataType(DataType.Date)] public DateOnly DateOfBirth { get; set; }
            [Required, EmailAddress] public string Email { get; set; }
            [Required] public string Phone { get; set; }
            [Required] public Gender Gender { get; set; }

            // Address Info
            [Required] public string BuildingNumber { get; set; }
            [Required] public string Street { get; set; }
            [Required] public string City { get; set; }
            [Required] public string FloorNumber { get; set; }

            // Appointment Info
            [Required] public int DoctorId { get; set; }
            [Required, DataType(DataType.Date)] public DateTime AppointmentDate { get; set; }
            public TimeSpan? SelectedTime { get; set; }

            // UI Helpers
            public List<ClinicManagemnetDAL.Models.Doctor> Doctors { get; set; } = new();
            public List<TimeSpan> AvailableSlots { get; set; } = new();
        }
    }


   

   
