using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AspNetIdentitySample.WebApi.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Client> Client { get; set; }

        public DbSet<Claims> Claims { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<AspNetIdentitySample.WebApi.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}
