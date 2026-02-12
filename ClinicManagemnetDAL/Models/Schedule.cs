using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsWorking { get; set; }

        #region Relationships
        #region Doctor - Schedule
        public int DoctorId { get; set; }                     // FK  
        public Doctor DoctorSchedule { get; set; } = null!;  // One Table Name

        #endregion
        #endregion

    }
}
