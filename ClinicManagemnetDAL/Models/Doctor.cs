using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class Doctor : User 
    {
        public int Id { get; set; }
        public string Specialization { get; set; } = null!;

        //Navigation Properties
        public ICollection<Schedule> schedules { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = null!;

        #region Relationships
        #region Address - Doctor
        public int AddressId { get; set; }                    // FK
        public Address Address { get; set; } = null!; // One Table Name
        #endregion

        #region Clinic - Doctor
        public int ClinicId { get; set; }            // FK
        public Clinic Clinic { get; set; } = null!; // One Table Name
        #endregion
        #endregion
    }
}
