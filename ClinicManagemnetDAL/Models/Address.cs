using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string BuildingNumber { get; set; } = null!;
        public string FloorNumber { get; set; } = null!;

        //Navigation Properties
        public ICollection<Doctor> Doctors { get; set; } = null!;
        public ICollection<Patient> Patients { get; set; } = null!;
        public ICollection<Clinic> Clinics { get; set; } = null!;

    }
}
