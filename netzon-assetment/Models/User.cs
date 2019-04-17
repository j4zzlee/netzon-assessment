using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netzon_assetment.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Column("firstname")]
        [Required]
        public string FirstName { get; set; }
        [Column("lastname")]
        [Required]
        public string LastName { get; set; }
        [Column("email")]
        [Required]
        public string Email { get; set; }
        [Column("password")]
        [Required]
        public string Password { get; set; }
        [Column("token")]
        public string Token { get; set; }

        public List<UserRole> Roles { get; set; }
    }
}
