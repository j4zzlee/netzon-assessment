using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.Models.Maps
{
    public class RoleMap
    {
        public RoleMap(EntityTypeBuilder<Role> builder)
        {
            builder.HasIndex(i => i.Code).IsUnique();
        }
    }
}
