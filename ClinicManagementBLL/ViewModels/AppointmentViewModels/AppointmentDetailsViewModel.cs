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
        // Patient
        public string PatientName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;

        // Address
        public string BuildingNumber { get; set; } = null!;
        public string FloorNumber { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;

        // Doctor & Appointment
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }


    }
}
