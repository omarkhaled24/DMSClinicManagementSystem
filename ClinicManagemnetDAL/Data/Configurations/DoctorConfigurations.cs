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
    internal class DoctorConfigurations : UserConfigurations<Doctor>, IEntityTypeConfiguration<Doctor>
    {
        public new void Configure(EntityTypeBuilder<Doctor> builder)
        {

            builder.HasOne(navigationExpression: X => X.Address)
                   .WithMany(navigationExpression: X => X.Doctors)
                   .HasForeignKey(foreignKeyExpression: X => X.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(navigationExpression: X => X.Clinic)
                  .WithMany(navigationExpression: X => X.Doctors)
                  .HasForeignKey(foreignKeyExpression: X => X.ClinicId);
            base.Configure(builder);

        }
    }
}
