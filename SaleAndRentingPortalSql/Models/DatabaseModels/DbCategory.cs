using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbCategory
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DbCategory()
        {

        }

        public DbCategory(string id, string category, DateTime created)
        {
            Id = id;
            Category = category;
            Created = created;
        }
    }
}
