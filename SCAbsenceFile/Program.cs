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
                List<string> grades = new List<string>() { "pk", "k", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };

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
                            date = Helpers.GetPreviousBusinessDay(DateTime.Today).ToString();
                        }
                        if (date.ToLower().Equals("now"))
                        {
                            date = DateTime.Now.ToString();
                        }
                    }
                    else if (argument.ToLower().StartsWith("/grades:"))
                    {
                        string gradesRaw = argument.Substring(8, argument.Length - 8).Trim('\"').Trim();
                        grades = new List<string>();
                        Logging.Info("Limiting student selection to specific grades");
                        foreach (string ss in gradesRaw.Split(','))
                        {
                            if (!string.IsNullOrEmpty(ss))
                            {
                                grades.Add(ss.ToLower());
                                Logging.Info(" Adding grade \"" + ss + "\"");
                            }
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
                        DateTime parsedDate = Helpers.ParseDate(date);

                        try
                        {
                            Logging.ToLog("----------------------------------------------------------------");
                            Logging.Info("Creating absence file for school " + parsedSchoolID + " for date " + parsedDate.ToLongDateString());

                            Dictionary<Student, List<Absence>> studentsWithAbsences = new Dictionary<Student, List<Absence>>();
                            using (SqlConnection connection = new SqlConnection(Config.dbConnectionString_SchoolLogic))
                            {
                                Logging.Info("Loading students");
                                List<Student> schoolStudents = Student.LoadForSchool(connection, parsedSchoolID,
                                     parsedDate).Where(s => grades.Contains(s.Grade.ToLower())).ToList();

                                Logging.Info("Loaded " + schoolStudents.Count + " students for school " + parsedSchoolID);

                                // Load student absences
                                Logging.Info("Loading absences");
                                foreach (Student student in schoolStudents)
                                {
                                    studentsWithAbsences.Add(student, Absence.LoadAbsencesFor(connection, student, parsedDate, parsedDate.AddHours(23.5)));
                                }
                            }

                            Logging.Info("Creating CSV data");

                            MemoryStream csvContents = AbsenceCSV.GenerateCSV(studentsWithAbsences);
                            Logging.Info("Saving CSV file (" + fileName + ")");
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
