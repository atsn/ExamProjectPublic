using System;
using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbPictures
    {
        [StringLength(40)]
        [Required]
        public string ProductId { get; set; }

        [StringLength(100)]
        [Required]
        [Key]
        public string Path { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DbPictures()
        {

        }

        public DbPictures(string productId, string path, DateTime created)
        {
            ProductId = productId;
            Path = path;
            Created = created;
        }


    }
}
