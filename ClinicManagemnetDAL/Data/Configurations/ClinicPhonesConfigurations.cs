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
    internal class ClinicPhonesConfigurations : IEntityTypeConfiguration<ClinicPhones>
    {
        public void Configure(EntityTypeBuilder<ClinicPhones> builder)
        {
            builder.Property(X => X.PhoneNumber)
               .IsRequired()
               .HasMaxLength(20);

            builder.HasOne(navigationExpression: X => X.Clinic)
               .WithMany(navigationExpression: X => X.ClinicPhones)
               .HasForeignKey(foreignKeyExpression: X => X.ClinicId);
        }
    }
}
