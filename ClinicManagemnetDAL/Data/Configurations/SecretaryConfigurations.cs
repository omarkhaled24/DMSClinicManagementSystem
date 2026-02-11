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
    internal class SecretaryConfigurations :UserConfigurations<Secretary>, IEntityTypeConfiguration<Secretary>
    {
        public new void Configure(EntityTypeBuilder<Secretary> builder)
        {
            base.Configure(builder);
        }
    }
}
