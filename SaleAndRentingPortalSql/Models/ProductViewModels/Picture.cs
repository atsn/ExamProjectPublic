using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SaleAndRentingPortalSql.Models.ProductViewModels
{
    public class Picture
    {

       [Display(Name = "Sti")]
        public string Path { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
