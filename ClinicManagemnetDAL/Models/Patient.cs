using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
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
