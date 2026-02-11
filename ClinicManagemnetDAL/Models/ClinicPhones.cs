using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class ClinicPhones
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = null!;

        #region Relationships
        #region Clinic - Clinic Phones
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; } = null!;
        #endregion
        #endregion
    }
}
