using backend.Models;
using System.Collections.Generic;
using System.Linq;

namespace backend.Helpers
{
    public class PresentsHelper
    {
        public static List<string> Presents { get; private set; }

        public static void ExtractAllPresents(IEnumerable<Person> persons)
        {
            Presents = persons.Select(person => person.Present).Distinct().OrderBy(present => present).Where(present => present != null).ToList();
        }
    }
}
