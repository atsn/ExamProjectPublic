using SaleAndRentingPortalSql.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleAndRentingPortalSql.Models.ProductViewModels
{
    public class Product
    {
        [Required]
        [Display(Name = "Pris")]
        public int Price { get; set; }

        public string Id { get; set; }

        [Required]
        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Beskrivelse")]

        public string Description { get; set; }
        [Display(Name = "Oprettet")]
        public DateTime Created { get; set; }

        [NotMapped]
        public List<Specs> Specs { get; set; }

        [Required]
       [Display(Name = "Antal på lager")]
        public int NoOfItems { get; set; }

        [NotMapped]
        [Required]
        public List<string> Categories { get; set; }
        [NotMapped]
        public List<Picture> pictures { get; set; }

        public int status { get; set; } = 0;

        public bool IsInStock()
        {
            return NoOfItems > 0;
        }

        public Product()
        {

            Specs = new List<Specs>();
            Categories = new List<string>();
        }
        public Product(string id, int price, string name, string description, int noOfItems)
        {
            Id = id;
            Price = price;
            Name = name;
            Description = description;
            NoOfItems = noOfItems;

            Specs = new List<Specs>();
            Categories = new List<string>();
        }

        public Product(DbProductc product)
        {
            Id = product.Id;
            Price = product.Price;
            Name = product.Name;
            Description = product.Description;
            NoOfItems = product.NoOfItems;

            Specs = new List<Specs>();
            Categories = new List<string>();
        }
    }
}
