using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SaleAndRentingPortalSql.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public bool IsActive { get; set; } = true;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        public int Zipcode { get; set; }

    }
}
