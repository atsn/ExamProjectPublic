using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SaleAndRentingPortalSql.Models;
using SaleAndRentingPortalSql.Models.DatabaseModels;

namespace SaleAndRentingPortalSql.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbZipCodes> ZipCodes { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }

        public DbSet<DbOrdre> Ordre { get; set; }
        public DbSet<DbProductc> Product { get; set; }
        public DbSet<DbCategory> Category { get; set; }
        public DbSet<DbProductCategory> ProductCategory { get; set; }
        public DbSet<DbPictures> Pictures { get; set; }
        public DbSet<DbSpecs> Specs { get; set; }
        public DbSet<DbOrdreProduct> OrderProduct { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DbZipCodes>().ToTable("Zipcodes");
            builder.Entity<DbProductCategory>().HasKey(c => new { c.ProductId, c.CategoryId });
        }

        public DbSet<SaleAndRentingPortalSql.Models.ApplicationUser> ApplicationUser { get; set; }

        public DbSet<SaleAndRentingPortalSql.Models.AccountViewModels.EditUserViewModel> EditUserViewModel { get; set; }

        //public DbSet<SaleAndRentingPortalSql.Models.DatabaseModels.DbOrdre> DbOrdre { get; set; }
    }


}
