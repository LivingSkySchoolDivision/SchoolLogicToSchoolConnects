using System;
using System.IO;
using System.Text;

namespace SLDataLib
{
    public class Logging
    {
        public string LogFileName { get; set; }

        public Logging()
        {
            this.LogFileName = "scr_log_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".log";
        }

        public Logging(string fileName)
        {
            this.LogFileName = fileName;
        }
        
        public void Error(string message)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + message);
            Console.ForegroundColor = previousColor;
            ToLog("ERROR: " + message);
        }

        public void Info(string message)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("INFO: " + message);
            Console.ForegroundColor = previousColor;
            ToLog("INFO: " + message);
        }
        
        public void ToLog(string message)
        {
            using (FileStream logfileStream = new FileStream(LogFileName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(logfileStream, Encoding.UTF8))
                {
                    writer.WriteLine(DateTime.Now.ToShortDateString() +" " + DateTime.Now.ToLongTimeString() + " > " + message);
                }
            }
        }

    }
}
