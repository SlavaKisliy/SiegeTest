using Microsoft.EntityFrameworkCore;
using CustomerAPI.Models;

namespace CustomerAPI.Data
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        { 
        }
        public DbSet<Customer> CustomerItems {get; set;}
    }
}