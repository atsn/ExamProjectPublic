using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SaleAndRentingPortalSql.Models.DatabaseModels;

namespace SaleAndRentingPortalSql.Models.ProductViewModels
{
    public class OrdreProductViewModel2
    {
        public DbOrdre Ordre { get; set; }

        public List<Product> Products { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
