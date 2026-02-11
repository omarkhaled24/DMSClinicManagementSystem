using ClinicManagemnetDAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Data.Contexts
{
    public class ClinicDbContext : DbContext
    { 
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
        {

        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
        }

        #region DbSets
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ClinicPhones> ClinicPhones { get; set; }

        #endregion
    }
}
