using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.ProductViewModels
{
    public class Specs
    {
       [Display(Name = "Specifikation")]
        public string Text { get; set; }
        [Display(Name = "Værdi")]
       public string Value { get; set; }
    }
}
