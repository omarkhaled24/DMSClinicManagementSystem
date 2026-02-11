using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class Patient : User 
    {
        public int Id { get; set; }


        //Navigation Properties

        public ICollection<Appointment> Appointments { get; set; } = null!;
        #region Relationships 
        #region Address - Patient
        public int AddressId { get; set; }
        public Address Address { get; set; } = null!;
        #endregion
        #endregion
    }
}
