using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Indtast venligst din Email")]
        [EmailAddress(ErrorMessage = "Dette er ikke en gyldig Email adresse")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Indtast venligst dit fornavn")]
        [Display(Name = "Fornavn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Indtast venligst dit efternavn")]
        [Display(Name = "Efternavn")]
        public string LastName { get; set; }

        [Phone(ErrorMessage ="Skriv venligst et gyldigt telefonnummer")]
        [Display(Name = "Telefon Nummer")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Bekræft at du ikke er en robot")]
        public string RecaptchaResponse { get; set; }

        // [Required(ErrorMessage = "Indtast venligst din adresse")]
        [Display(Name = "Addresse")]
        [Required(ErrorMessage = "Indtast venligst en gyldig adresse")]

        public string Address { get; set; }


        // [Required(ErrorMessage = "Indtast venligst din adresse")]
        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Indtast venligst et gyldigt postnummer")]

        public int ZipCode { get; set; }

        // [Required(ErrorMessage = "Indtast venligst din adresse")]
        [Display(Name = "By")]
        [Required(ErrorMessage = "Indtast venligst et gyldigt postmunner for at finde din by")]
        public string City { get; set; }

        [Required(ErrorMessage = "Indtast venligst dit kodeord")]
        [StringLength(30, ErrorMessage = "Kodeordet skal være mindst {2} og max {1} karakterer langt.", MinimumLength = 8)]
        [DataType(DataType.Password, ErrorMessage = "Dit kodeord indeholde et tal, et stort og et lille bogstav ")]
        [Display(Name = "Kodeord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekræft kodeord")]
        [Compare("Password", ErrorMessage = "Dit kodeord og bekræft kodeord stemmer ikke over ens.")]
        public string ConfirmPassword { get; set; }
    }
}
