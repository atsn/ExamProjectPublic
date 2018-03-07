using Microsoft.AspNetCore.Identity;

namespace SaleAndRentingPortalSql.Models
{
    public class ApplicationRole : IdentityRole
    {

        public ApplicationRole(string rolename)
        {
            this.Name = rolename;

        }

        public ApplicationRole()
        {

        }
    }

}
