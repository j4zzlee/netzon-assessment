using System.ComponentModel.DataAnnotations;

namespace netzon_assetment.ViewModels
{
    public class UserLoginViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
