using SaleAndRentingPortalSql.Models.ProductViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbProductc
    {



        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        public int Price { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [StringLength(4000)]
        public String Description { get; set; }

        [NotMapped]
        public List<DbProductCategory> ProductCategory { get; set; }
        [NotMapped]
        public List<DbSpecs> Specs { get; set; }

        [Required]
        public int NoOfItems { get; set; }

        public DbProductc()
        {
             
        }

        public DbProductc(string id, int price, string name, DateTime created, string description, int noOfItems)
        {
            Id = id;
            Price = price;
            Name = name;
            Created = created;
            Description = description;
            NoOfItems = noOfItems;
        }
        public DbProductc(Product product)
        {
            Id = product.Id;
            Price = product.Price;
            Name = product.Name;
            Created = product.Created;
            Description = product.Description;
            NoOfItems = product.NoOfItems;
        }
    }
}
