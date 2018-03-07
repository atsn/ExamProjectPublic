using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.ProductViewModels
{
    public class Productsearchmodel
    {
        [Display(Name ="Søg efter vare")]
        public string searchString { get; set; }

        [Display(Name = "Min Pris")]
        public int? minprice { get; set; }

        [Display(Name = "Max pris")]
        public int? maxprice { get; set; }


        public bool isstock { get; set; } = false;

        public List<Product> products;

        public Productsearchmodel()
        {
            products = new List<Product>();
        }
    }
}
