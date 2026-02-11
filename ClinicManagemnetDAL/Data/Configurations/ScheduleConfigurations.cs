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
    internal class ScheduleConfigurations : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasOne(navigationExpression: X => X.DoctorSchedule)
                .WithMany(navigationExpression: X => X.schedules)
                .HasForeignKey(foreignKeyExpression: X => X.DoctorId);
        }
    }
}
