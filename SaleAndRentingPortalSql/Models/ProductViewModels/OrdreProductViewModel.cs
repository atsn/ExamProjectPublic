using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.ProductViewModels
{
    public class OrdreProductViewModel
    {
        public OrdreViewModel Ordre { get; set; }

        public List<Product> Products { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
