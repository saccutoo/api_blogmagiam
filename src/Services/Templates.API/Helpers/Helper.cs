using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Templates.API
{
    public static class HelperCommon
    {
        public static List<DateTime> GetListDateinMonth(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day)) // Map each day to a date
                             .ToList(); // Load dates into a list
        }
    }
}
