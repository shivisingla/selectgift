using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithmForPresentPrediction.DB
{
    public class PersonDbContext : DbContext
    {
        public DbSet<Gen> Persons { get; set; }

        public PersonDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=EPUALVIW005B;Database=UniQueDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }

        public IEnumerable<Gen> GetTrainingDataSet(IEnumerable<Gen> testingDataSet)
        {
            return Persons.Except(testingDataSet);
        }

        public IEnumerable<Gen> GetTestingDataSet()
        {
            int tenPercentsOfPersonsCount = Persons.Count() / 10;
            return Persons.Distinct().OrderBy(arg => Guid.NewGuid()).Take(tenPercentsOfPersonsCount);
        }
    }
}
