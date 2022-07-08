using ASPNET6WebApiDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPNET6WebApiDemo.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
    }
}
