using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLDataLib
{
    public static class Extensions
    {
        public static string ToCommaSeparatedString<T>(this List<T> list)
        {
            StringBuilder returnMe = new StringBuilder();

            foreach (T item in list)
            {
                returnMe.Append(item);
                returnMe.Append(", ");
            }

            if (returnMe.Length > 2)
            {
                returnMe.Remove(returnMe.Length - 2, 2);
            }

            return returnMe.ToString();
        }

    }
}
