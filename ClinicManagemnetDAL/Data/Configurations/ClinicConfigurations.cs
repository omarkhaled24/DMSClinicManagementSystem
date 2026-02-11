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
    internal class ClinicConfigurations : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.HasOne(navigationExpression: X => X.Address)
                .WithMany(navigationExpression: X => X.Clinics)
                .HasForeignKey(foreignKeyExpression: X => X.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
