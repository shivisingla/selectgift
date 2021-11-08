using System.Collections.Generic;
using System.Linq;

namespace backend.Helpers
{
    public static class DropdownHelper
    {
        public static List<string> GetDistinctValues(List<string> values)
        {
            values = values.Where(value => value != "Other")
                .Distinct()
                .OrderBy(relation => relation)
                .ToList();

            values.Add("Other");
            return values;
        }

        public static List<string> GetDistinctSplittedValues(List<string> values)
        {
            var splittedValues = new List<string>();
            foreach (var value in values)
            {
                splittedValues.AddRange(value.Split(','));
            }

            return GetDistinctValues(splittedValues);
        }
    }
}
