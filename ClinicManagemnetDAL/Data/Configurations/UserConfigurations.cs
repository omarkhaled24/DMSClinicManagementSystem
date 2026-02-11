using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagemnetDAL.Models;

namespace ClinicManagemnetDAL.Data.Configurations
{
    internal class UserConfigurations<T> : IEntityTypeConfiguration<T> where T : User
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(propertyExpression: X => X.Name)
                  .HasColumnType(typeName: "varchar")
                  .HasMaxLength(maxLength: 50);
              
            builder.Property(propertyExpression: X => X.Email)
                  .HasColumnType(typeName: "varchar")
                  .HasMaxLength(maxLength: 100);

            builder.Property(propertyExpression: X => X.PhoneNumber)
                  .HasColumnType(typeName: "varchar")
                  .HasMaxLength(maxLength: 11);


            builder.Property(X => X.DateOfBirth)
                   .IsRequired()
                   .HasAnnotation("CheckConstraint", "[DateOfBirth] <= GETDATE()");

            builder.ToTable(buildAction: Tb =>
            {
                Tb.HasCheckConstraint(name: "GymUserValidEmailCheck", sql: "Email Like '_%@_%._%'");
                Tb.HasCheckConstraint(name: "GymUserValidPhoneNumberCheck", sql: "PhoneNumber Like '01%' and PhoneNumber Not Like '%[^0-9]%'");
            });
            builder.HasIndex(indexExpression: X => X.Email).IsUnique();
            builder.HasIndex(indexExpression: X => X.PhoneNumber).IsUnique();

        }
    }
}

