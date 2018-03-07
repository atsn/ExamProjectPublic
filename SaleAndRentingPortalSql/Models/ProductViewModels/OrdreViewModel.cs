using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models.ProductViewModels
{
    public class OrdreViewModel
    {

        [Key]
        public string Token { get; set; }
        
        [Display(Name = "Pris")]
        public int Price { get; set; }

        [Display(Name = "Dato")]
        public string OrderDate { get; set; }

        [Display(Name = "TidsPunkt")]
        public string OrderTime { get; set; }

        [Required]
        public string KortholderNavn { get; set; }

        [StringLength(100)]
        [Required]
        [Display(Name = "Adresse")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Postnummer")]
        public int Zipcode { get; set; }

        [Required]
        public string Email { get; set; }

        public OrdreViewModel()
        {
            
        }

        public OrdreViewModel(string token, int price, string orderDate, string orderTime, string kortholderNavn, string address, int zipcode, string email)
        {
            Token = token;
            Price = price;
            OrderDate = orderDate;
            OrderTime = orderTime;
            KortholderNavn = kortholderNavn;
            Address = address;
            Zipcode = zipcode;
            Email = email;
        }
    }
}
