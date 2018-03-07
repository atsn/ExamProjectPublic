using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbZipCodes
    {
        [Key]
        public int ZipCode { get; set; }
        public string City { get; set; }

        public DbZipCodes()
        {

        }
    }
}
