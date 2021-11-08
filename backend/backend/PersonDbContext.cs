using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class PersonDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public PersonDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=EPUALVIW005B;Database=UniQueDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }
    }
}
