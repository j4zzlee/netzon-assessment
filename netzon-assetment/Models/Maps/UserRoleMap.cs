using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.Models.Maps
{
    public class UserRoleMap
    {
        public UserRoleMap(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(o => new { o.UserID, o.RoleID });
            builder
               .HasOne(ur => ur.User)
               .WithMany(u => u.Roles)
               .HasForeignKey(ur => ur.UserID);

            builder
               .HasOne(ur => ur.Role)
               .WithMany(u => u.Users)
               .HasForeignKey(ur => ur.RoleID);
        }
    }
}
