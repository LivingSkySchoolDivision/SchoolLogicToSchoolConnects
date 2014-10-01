using System;

namespace SLDataLib
{
    public static class Logging
    {
        public static void Error(string message)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + message);
            Console.ForegroundColor = previousColor;
        }

        public static void Info(string message)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("INFO: " + message);
            Console.ForegroundColor = previousColor;
        }

        public static void Success(string message)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SUCCESS: " + message);
            Console.ForegroundColor = previousColor;
        }

        public static void ToLog(string message)
        {
            
        }

    }
}
