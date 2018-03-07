using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "BrugerNavn")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

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

        [Phone(ErrorMessage = "Scriv veligst et gyldigt telefonnummer")]
        [Display(Name = "Telefon Nummer")]
        public string PhoneNumber { get; set; }


        // [Required(ErrorMessage = "Indtast venligst din adresse")]
        [Display(Name = "Addresse")]
        [Required(ErrorMessage = "Indtast venligst en gyldig adresse")]

        public string Address { get; set; }


        // [Required(ErrorMessage = "Indtast venligst din adresse")]
        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Indtast venligst et gyldigt postmunner")]

        public int ZipCode { get; set; }

        // [Required(ErrorMessage = "Indtast venligst din adresse")]
        [Display(Name = "By")]
        [Required(ErrorMessage = "Indtast venligst et gyldigt postmunner for at finde din by")]
        public string City { get; set; }

        public string StatusMessage { get; set; }
    }
}
