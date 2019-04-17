using Microsoft.EntityFrameworkCore;
using netzon_assetment.Models.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.Models
{
    public class NetzonDbContext: DbContext
    {
        public NetzonDbContext(DbContextOptions<NetzonDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            new UserMap(builder.Entity<User>());
            new RoleMap(builder.Entity<Role>());
            new UserRoleMap(builder.Entity<UserRole>());
        }
    }
}
