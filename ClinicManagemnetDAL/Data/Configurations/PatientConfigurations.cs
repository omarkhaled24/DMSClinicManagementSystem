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
    internal class PatientConfigurations : UserConfigurations<Patient>, IEntityTypeConfiguration<Patient>
    {
        public new void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasOne(navigationExpression: X => X.Address)
                    .WithMany(navigationExpression: X => X.Patients)
                    .HasForeignKey(foreignKeyExpression: X => X.AddressId);
            base.Configure(builder);

        }
    }
}
