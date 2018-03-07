using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbOrdreProduct
    {
        [Key]
        public string Id { get; set; }

        public string OrderId { get; set; }

        [StringLength(40)]
        public string ProductId { get; set; }

        [NotMapped]
        public DbProductc Productc { get; set; }

        [NotMapped]
        public DbOrdre Ordre { get; set; }

        public DbOrdreProduct()
        {

        }

        public DbOrdreProduct(string id, string orderid, string prductid)
        {
            Id = id;
            OrderId = orderid;
            ProductId = prductid;
        }
    }
}
