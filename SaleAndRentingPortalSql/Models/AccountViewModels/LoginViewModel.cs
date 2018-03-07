using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Indtast venligst Email")]
        [EmailAddress(ErrorMessage = "Dette er ikke en gyldig Email adresse")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Indtast venligst kodeord")]
        [StringLength(30, ErrorMessage = "Kodeordet skal være mindst {2} og max {1} karakterer langt.", MinimumLength = 8)]
        [DataType(DataType.Password, ErrorMessage ="Dit kodeord indeholde et tal, et stort og et lille bogstav " )]
        [Display(Name = "Kodeord")]
        public string Password { get; set; }

        [Display(Name = "Husk login")]
        public bool RememberMe { get; set; }
    }
}
