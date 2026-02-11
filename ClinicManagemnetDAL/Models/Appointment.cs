using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int VisitLength { get; set; } = 30;

        #region Relationships
        #region Doctor - Appointment
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        #endregion

        #region Patient - Appointment
        public int PatientId { get; set; }              // FK
        public Patient Patient { get; set; } = null!; // One Table Name
        #endregion

        #region Secretary - Appointment
        public int SecretaryId  { get; set; }
        public Secretary Secretary { get; set; } = null!;
        #endregion
        #endregion

    }
}
