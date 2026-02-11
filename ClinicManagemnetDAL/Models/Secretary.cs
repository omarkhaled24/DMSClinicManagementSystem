using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class Secretary : User
    {
        public int Id {  get; set; }

        //Navigation Properties

        public ICollection<Appointment> Appointments { get; set; } = null!;
    }
}
