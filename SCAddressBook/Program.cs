using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCAddressBook
{
    class Program
    {
        static void SendSyntax()
        {
            Console.WriteLine("Syntax goes here");
        }

        static void Main(string[] args)
        {
            if (args.Count() > 0)
            {

                // Load config file

                // Parse arguments
                // /schoolid:
                // /type:[addressbook|absences]
                // /filename:[absences.txt]
                // /date:[YYYYMMDD]
                // NEED: SFTP credentials

                string parsedSchoolID = string.Empty;
                string fileType = string.Empty;
                string fileName = string.Empty;
                string date = string.Empty;

                foreach (string argument in args)
                {
                    if (argument.ToLower().StartsWith("/schoolid"))
                    {
                        parsedSchoolID = "set";
                    }
                    else if (argument.ToLower().StartsWith("/type"))
                    {
                        fileType = "set";
                    }
                    else if (argument.ToLower().StartsWith("/filename"))
                    {
                        fileName = "set";
                    }
                    else if (argument.ToLower().StartsWith("/date"))
                    {
                        date = "set";
                    }
                }

                Logging.ToConsole("SchoolID: " + parsedSchoolID);
                Logging.ToConsole("Report Type: " + fileType);
                Logging.ToConsole("File Name: " + fileName);
                Logging.ToConsole("Date: " + date);

            }
            else
            {
                // If any argument is missing, send the syntax
                SendSyntax();
            }

        }
    }
}
