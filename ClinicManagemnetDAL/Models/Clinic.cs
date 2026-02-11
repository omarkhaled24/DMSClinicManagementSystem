using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Navigation Properties
        public ICollection<Doctor> Doctors { get; set; } = null!;  // Many Table Name
        public ICollection<ClinicPhones> ClinicPhones { get; set; } = null!;  // Many Table Name
        


        #region Relationships
        #region Address - Clinic
        public int AddressId { get; set; }
        public Address Address { get; set; } = null!;
        #endregion
        #endregion

    }
}
