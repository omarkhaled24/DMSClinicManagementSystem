using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementBLL.ViewModels.PatientViewModel
{
    public class PatientDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Doctor { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Age { get; set; }

        // Address
        public string BuildingNumber { get; set; } = null!;
        public string FloorNumber { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;

        // Appointment
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public string SelectedTime { get; set; } = null!;


    }
}
