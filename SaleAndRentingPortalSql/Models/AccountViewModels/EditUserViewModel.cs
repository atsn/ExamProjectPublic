using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleAndRentingPortalSql.Models.AccountViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Indtast venligst Email")]
        [EmailAddress(ErrorMessage = "Dette er ikke en gyldig Email adresse")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Skriv venligst et gyldigt telefonnummer")]
        [Display(Name = "Telefon Nummer")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Indtast venligst dit fornavn")]
        [Display(Name = "Fornavn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Indtast venligst dit efternavn")]
        [Display(Name = "Efternavn")]
        public string LastName { get; set; }

        [Display(Name = "Addresse")]
        [Required]
        public string Address { get; set; }

        [Display(Name = "Email bekræftet?")]
        [Required]
        public bool IsConfirmed { get; set; }

        [Display(Name = "Postnummer")]
        [Required]
        public int Zipcode { get; set; }

        [Display(Name = "Rolle")]
        [Required]
        public string Role { get; set; }

       
        [StringLength(30, ErrorMessage = "Kodeordet skal være mindst {2} og max {1} karakterer langt.", MinimumLength = 8)]
        [DataType(DataType.Password, ErrorMessage = "Dit kodeord indeholde et tal, et stort og et lille bogstav ")]
        [Display(Name = "Nyt Kodeord")]
        public string NewPassword { get; set; }



        public bool Isactive { get; set; }

        [NotMapped]
        public List<SelectListItem> AllRoles { get; set; }

        public EditUserViewModel(ApplicationUser user)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.IsConfirmed = user.EmailConfirmed;
            this.PhoneNumber = user.PhoneNumber;
            this.Zipcode = user.Zipcode;
            this.Email = user.Email;
            this.Address = user.Address;
            this.Isactive = user.IsActive;
        }

        public ApplicationUser MakeApplicationUser(ApplicationUser user)
        {
            user.Id = this.Id;
            user.FirstName = this.FirstName;
            user.LastName = this.LastName;
            user.EmailConfirmed = this.IsConfirmed;
            user.PhoneNumber = this.PhoneNumber;
            user.Zipcode = this.Zipcode;
            user.Email = this.Email.ToLower();
            user.UserName = this.Email.ToLower();
            user.Address = this.Address;
            user.IsActive = this.Isactive;
            return user;
        }

        public EditUserViewModel()
        {
            AllRoles = new List<SelectListItem>();
        }
    }


}
