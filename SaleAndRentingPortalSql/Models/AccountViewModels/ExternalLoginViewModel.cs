using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Dette er ikke en gyldig Email adresse")]
        public string Email { get; set; }
    }
}
