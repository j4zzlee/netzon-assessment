using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.ViewModels
{
    public class UserRegistrationViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Firstname { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Lastname { get; set; }
        [Required]
        [EmailAddress]

        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
