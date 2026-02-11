using ClinicManagemnetDAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagemnetDAL.Data.Configurations
{
    internal class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasOne(navigationExpression: X => X.Doctor)
                 .WithMany(navigationExpression: X => X.Appointments)
                 .HasForeignKey(foreignKeyExpression: X => X.DoctorId);

            builder.HasOne(navigationExpression: X => X.Patient)
                .WithMany(navigationExpression: X => X.Appointments)
                .HasForeignKey(foreignKeyExpression: X => X.PatientId);

            builder.HasOne(navigationExpression: X => X.Secretary)
               .WithMany(navigationExpression: X => X.Appointments)
               .HasForeignKey(foreignKeyExpression: X => X.SecretaryId);
        }
    }
}
