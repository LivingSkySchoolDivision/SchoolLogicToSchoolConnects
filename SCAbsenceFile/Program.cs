using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLDataLib;

namespace SCAbsenceFile
{
    class Program
    {
        static void SendSyntax()
        {
            Console.WriteLine("Syntax goes here");
        }

        static void Main(string[] args)
        {
            if (args.Any())
            {
                string parsedSchoolID = string.Empty;
                string fileName = string.Empty;
                string date = string.Empty;

                foreach (string argument in args)
                {
                    if (argument.ToLower().StartsWith("/schoolid:"))
                    {
                        parsedSchoolID = argument.Substring(10, argument.Length - 10);
                    }
                    else if (argument.ToLower().StartsWith("/filename:"))
                    {
                        fileName = argument.Substring(10, argument.Length - 10);
                    }
                    else if (argument.ToLower().StartsWith("/date:"))
                    {
                        date = argument.Substring(6, argument.Length - 6);
                        if (date.ToLower().Equals("today"))
                        {
                            date = DateTime.Today.ToString();
                        }
                        if (date.ToLower().Equals("yesterday"))
                        {
                            date = DateTime.Today.AddDays(-1).ToString();
                        }
                        if (date.ToLower().Equals("now"))
                        {
                            date = DateTime.Now.ToString();
                        }
                    }
                }

                if ((String.IsNullOrEmpty(parsedSchoolID)) || (string.IsNullOrEmpty(fileName)) || (string.IsNullOrEmpty(date)))
                {
                    SendSyntax();
                }
                else
                {
                    if (Config.ConfigFileExists())
                    {
                        // Do things
                        DateTime parsedDate = Helpers.ParseDate(date);

                        try
                        {
                            Dictionary<Student, List<Absence>> studentsWithAbsences = new Dictionary<Student, List<Absence>>();
                            using (SqlConnection connection = new SqlConnection(Config.dbConnectionString_SchoolLogic))
                            {
                                Logging.Info("Loading students...");
                                List<Student> schoolStudents = Student.LoadForSchool(connection, parsedSchoolID,
                                     parsedDate);

                                // Load student absences
                                Logging.Info("Loading absences...");
                                foreach (Student student in schoolStudents)
                                {
                                    studentsWithAbsences.Add(student, Absence.LoadAbsencesFor(connection, student, parsedDate, parsedDate.AddHours(23.5)));
                                }
                            }

                            Logging.Info("Loaded " + studentsWithAbsences.Count + " students for school " + parsedSchoolID);
                            Logging.Info("Creating CSV...");

                            MemoryStream csvContents = AbsenceCSV.GenerateCSV(studentsWithAbsences);
                            Logging.Info("Saving CSV...");
                            if (FileHelpers.FileExists(fileName))
                            {
                                FileHelpers.DeleteFile(fileName);
                            }
                            FileHelpers.SaveFile(csvContents, fileName);
                            Logging.Info("Done!");

                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex.Message);
                        }
                    }
                    else
                    {
                        Logging.Error("Configuration file not found");
                        Logging.Info("Creating new config file (" + Config.configFileName + ")...");
                        Logging.Info("Please edit the file and try again");
                        Config.CreateNewConfigFile();
                    }
                }

            }
            else
            {
                // If any argument is missing, send the syntax
                SendSyntax();
            }

        }
    }
}
