using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbProductCategory
    {

        [Required]
        [StringLength(40)]
        [Key]
        public string ProductId { get; set; }

        [Required]
        [StringLength(50)]
        [Key]
        public string CategoryId { get; set; }

        public DbProductc Product { get; set; }

        public DbProductCategory()
        {

        }

        public DbProductCategory(string productId, string categoryId)
        {
            ProductId = productId;
            CategoryId = categoryId;
        }

    }
}
