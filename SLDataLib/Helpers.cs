using System;

namespace SLDataLib
{
    public static class Helpers
    {
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

    }
}
