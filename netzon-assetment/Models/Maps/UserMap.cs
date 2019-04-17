using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.Models.Maps
{
    public class UserMap
    {
        public UserMap(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(i => i.Email).IsUnique();
        }
    }
}
