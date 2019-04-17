using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("code")]
        [Required]
        public string Code { get; set; }
        [Column("label")]
        [Required]
        public string Label { get; set; }

        public List<UserRole> Users { get; set; }
    }
}
