using System;
using System.IO;
using System.Text;

namespace SLDataLib
{
    public static class Logging
    {
        /// <summary>
        /// Returns the name of the log file to use
        /// </summary>
        public static string LogfileName()
        {
            return "scr_log_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".log";
        }
        
        public static void Error(string message)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + message);
            Console.ForegroundColor = previousColor;
            ToLog("ERROR: " + message);
        }

        public static void Info(string message)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("INFO: " + message);
            Console.ForegroundColor = previousColor;
            ToLog("INFO: " + message);
        }
        
        public static void ToLog(string message)
        {
            using (FileStream logfileStream = new FileStream(LogfileName(), FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(logfileStream, Encoding.UTF8))
                {
                    writer.WriteLine(DateTime.Now.ToShortDateString() +" " + DateTime.Now.ToLongTimeString() + " > " + message);
                }
            }
        }

    }
}
