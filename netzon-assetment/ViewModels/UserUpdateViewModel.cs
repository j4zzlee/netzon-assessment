using System.ComponentModel.DataAnnotations;

namespace netzon_assetment.ViewModels
{
    public class UserUpdateViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public int? UserID { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
    }
}
