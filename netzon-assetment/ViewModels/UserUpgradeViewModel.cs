using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.ViewModels
{
    public class UserUpgradeViewModel
    {
        [Required]

        public int? UserID { get; set; }
    }
}
