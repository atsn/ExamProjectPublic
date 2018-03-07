using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.DatabaseModels
{
    public class DbOrdre
    {
        [Display(Name = "Transaktions id")]
        public string Txnid { get; set; }

        [Key]
        [Display(Name = "Ordre id")]
        public string Orderid { get; set; }

        [Display(Name = "Antal Vare")]
        public int Amount { get; set; }

        [Display(Name = "Valuta")]
        public int Currency { get; set; }

        [Display(Name = "Dato")]

        public string OrderDate { get; set; }

        [Display(Name = "Tidspunkt")]
        public string OrderTime { get; set; }

        [Display(Name = "Hash")]
        public string Hash { get; set; }

 
        [Required]
        [Display(Name = "BetalingsType")]
        public string Paymenttype { get; set; }

    

        [Display(Name = "Ordre Status")]
        public string Status { get; set; }

        [Required]
        public string KortholderNavn { get; set; }

        [StringLength(100)]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Display(Name = "Postnummer")]
        public int Zipcode { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "Bruger id")]
        public string UserId { get; set; }
        [Display(Name = "Pris")]
        public int Price { get; set; }


        public List<DbOrdreProduct> OrderProducts { get; set; }

        public DbOrdre()
        {

        }

        public DbOrdre(string txnid, string orderid, int amount, int currency, string orderDate, string orderTime, string hash, string paymenttype, string status, string kortholderNavn, string address, int zipcode, string email, string userId, List<DbOrdreProduct> orderProducts)
        {
            Txnid = txnid;
            Orderid = orderid;
            Amount = amount;
            Currency = currency;
            OrderDate = orderDate;
            OrderTime = orderTime;
            Hash = hash;
            Paymenttype = paymenttype;
            Status = status;
            KortholderNavn = kortholderNavn;
            Address = address;
            Zipcode = zipcode;
            Email = email;
            UserId = userId;
            OrderProducts = orderProducts;
        }
    }
}
