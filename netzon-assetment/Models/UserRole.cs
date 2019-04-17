using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.Models
{
    [Table("users_roles")]
    public class UserRole
    {
        [Column("user_id")]
        [Required]
        public int UserID { get; set; }
        [Column("role_id")]
        [Required]
        public int RoleID { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
