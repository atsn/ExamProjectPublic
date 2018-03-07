using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Indtast venligst dit kodeord")]
        [StringLength(30, ErrorMessage = "Kodeordet skal være mindst {2} og max {1} karakterer langt.", MinimumLength = 8)]
        [DataType(DataType.Password, ErrorMessage = "Dit kodeord indeholde et tal, et stort og et lille bogstav ")]
        [Display(Name = "Nyt Kodeord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekræft kodeord")]
        [Compare("Password", ErrorMessage = "Dit kodeord og bekræft kodeord stemmer ikke over ens.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
