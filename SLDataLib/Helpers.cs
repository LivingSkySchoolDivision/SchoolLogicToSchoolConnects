using System;
using System.Collections.Generic;
using System.Text;

namespace SLDataLib
{
    public static class Helpers
    {
        public static DateTime DatabaseNullDate = new DateTime(1900, 1, 1, 0, 0, 0);

        /// <summary>
        /// A potentially easier to use wrapper for the parse int function
        /// </summary>
        /// <param name="thisString"></param>
        /// <returns></returns>
        public static int ParseInt(string thisString)
        {
            int returnMe = 0;

            if (int.TryParse(thisString, out returnMe))
            {
                return returnMe;
            }

            return 0;
        }
        
        public static bool ParseBool(string thisDatabaseValue)
        {
            if (String.IsNullOrEmpty(thisDatabaseValue))
            {
                return false;
            }
            else
            {
                bool parsedBool = false;
                bool.TryParse(thisDatabaseValue, out parsedBool);
                return parsedBool;
            }
        }
        
        public static string BoolToYesOrNo(bool thisBool)
        {
            if (thisBool)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }
        
        public static DateTime ParseDate(string thisDate)
        {
            // Check if we were given the "null" date value from a SQL database, and return the C# minvalue instead
            if (thisDate.Trim() == "1900-01-01 00:00:00.000")
            {
                return DateTime.MinValue;
            }

            DateTime returnMe = DateTime.MinValue;

            if (!DateTime.TryParse(thisDate, out returnMe))
            {
                returnMe = DateTime.MinValue;
            }

            // Again, check if we've managed to parse the SQL's minimum date and convert it
            if (returnMe == new DateTime(1900, 1, 1))
            {
                returnMe = DateTime.MinValue;
            }

            return returnMe;
        }
        public static string FormatTelephoneNumber_JustNumbers(string value)
        {
            // Strip any non-numeric character out
            List<char> allowedChars = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            StringBuilder returnMe = new StringBuilder();

            foreach (char c in value)
            {
                if (allowedChars.Contains(c))
                {
                    returnMe.Append(c);
                }
            }

            if (returnMe.Length == 11)
            {
                if (returnMe[0] == '1')
                {
                    returnMe.Remove(0, 1);
                }
            }

            return returnMe.Length == 10 ? returnMe.ToString() : string.Empty;
        }

        public static bool ValidTelephoneNumber(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if ((FormatTelephoneNumber_JustNumbers(value).Length == 10) ||
                    (FormatTelephoneNumber_JustNumbers(value).Length == 11))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
