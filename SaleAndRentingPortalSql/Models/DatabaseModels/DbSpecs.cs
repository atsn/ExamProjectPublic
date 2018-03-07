using System;
using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbSpecs
    {

        [Required]
        [StringLength(50)]
        public string SpecName { get; set; }


        [Required]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        [Required]
        [StringLength(40)]
        public string ProductId { get; set; }

        public DbProductc Product { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DbSpecs()
        {
            
        }

        public DbSpecs(string specName, string id, string value, string productId, DateTime created)
        {
            SpecName = specName;
            Id = id;
            Value = value;
            ProductId = productId;
            Created = created;
        }




    }
}
