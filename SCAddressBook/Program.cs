using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLDataLib;
using SLDataLib.Repositories;

namespace SCAddressBook
{
    class Program
    {
        static Logging Log = new Logging();

        static void SendSyntax()
        {
            Console.WriteLine("SYNTAX:");
            Console.WriteLine("");
            Console.WriteLine(" REQUIRED:");
            Console.WriteLine("");
            Console.WriteLine(" /schoolid:000000");
            Console.WriteLine(" Specify the government ID of the school, as found in the \"code\" field in");
            Console.WriteLine(" SchoolLogic.");
            Console.WriteLine(" Required, if not using //allschools.");
            Console.WriteLine("");
            Console.WriteLine(" /filename:filename.csv");
            Console.WriteLine(" Specify the filename to use for the output");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" /date:[today|yesterday|now]");
            Console.WriteLine(" Which day would you like absences from.");
            Console.WriteLine("");
            Console.WriteLine(" OPTIONAL:");
            Console.WriteLine("");
            Console.WriteLine(" /grades:pk,k,1,2,3,4,5,6,7,8,9,10,11,12");
            Console.WriteLine("");
            Console.WriteLine(" /logfilename:filename.log");
            Console.WriteLine(" Specify the filename to use for the log");
            Console.WriteLine("");
            Console.WriteLine(" /allschools");
            Console.WriteLine(" Select all schools (ignoring //schoolid)");

        }

        static void Main(string[] args)
        {
            if (args.Any())
            {
                string fileName = string.Empty;
                string date = string.Empty;
                List<string> grades = new List<string>() { "pk", "k", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
                bool allSchools = false;
                List<string> options = new List<string>();

                List<string> selectedSchoolIDs = new List<string>();

                foreach (string argument in args)
                {
                    if (argument.ToLower().StartsWith("/schoolid:"))
                    {
                        foreach (string enteredID in argument.Substring(10, argument.Length - 10).Split(new char[] { ';', ',' }))
                        {
                            if (!string.IsNullOrEmpty(enteredID))
                            {
                                selectedSchoolIDs.Add(enteredID);
                            }
                        }
                    }
                    else if (argument.ToLower().StartsWith("/allschools"))
                    {
                        options.Add("allschools");
                        allSchools = true;
                    }
                    else if (argument.ToLower().StartsWith("/filename:"))
                    {
                        fileName = argument.Substring(10, argument.Length - 10);
                    }
                    else if (argument.ToLower().StartsWith("/logfilename:"))
                    {
                        Log.LogFileName = argument.Substring(13, argument.Length - 13);
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
                        Log.ToLog("Limiting student selection to specific grades");
                        foreach (string ss in gradesRaw.Split(','))
                        {
                            if (string.IsNullOrEmpty(ss))
                            {
                                grades.Add(ss.ToLower());

                                Log.ToLog(" Adding grade \"" + ss + "\"");
                            }
                        }
                    }
                }

                if (((selectedSchoolIDs.Count <= 0) && (!allSchools)) || (string.IsNullOrEmpty(fileName)) || (string.IsNullOrEmpty(date)))
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
                            Log.ToLog("----------------------------------------------------------------");
                            Log.Info("Started: " + DateTime.Now);
                            Log.Info("Date: " + date);
                            Log.Info("Output: " + fileName);
                            Log.Info("Grades: " + grades.ToCommaSeparatedString());
                            Log.Info("Options: " + options.ToCommaSeparatedString());
                            Log.Info("Schools: " + (allSchools ? "ALL" : selectedSchoolIDs.ToCommaSeparatedString()));

                            List<Student> reportStudents = new List<Student>();
                            List<School> selectedSchools = new List<School>();

                            ContactRepository contactRepo = new ContactRepository();
                            StudentRepository studentRepo = new StudentRepository();
                            using (SqlConnection connection = new SqlConnection(Config.dbConnectionString_SchoolLogic))
                            {
                                SchoolRepository schoolRepo = new SchoolRepository(connection);

                                if (allSchools)
                                {
                                    selectedSchools = schoolRepo.GetAll();
                                }
                                else
                                {
                                    selectedSchools = schoolRepo.Get(selectedSchoolIDs);
                                }

                                Log.Info("Loading students");
                                foreach (School school in selectedSchools)
                                {
                                    List<Student> schoolStudents = studentRepo.LoadForSchool(connection, school,
                                        parsedDate).Where(s => grades.Contains(s.Grade.ToLower())).ToList();

                                    Log.Info("Loaded " + schoolStudents.Count + " students for school " +
                                                 school.Name);

                                    // Load student contacts
                                    Log.Info("Loading student contacts");
                                    foreach (Student student in schoolStudents)
                                    {
                                        student.Contacts =contactRepo.LoadForStudent(connection, student.DatabaseID);
                                    }
                                    reportStudents.AddRange(schoolStudents);
                                }
                            }


                            Log.Info("Creating CSV data");
                            MemoryStream csvContents = AddressBookCSV.GenerateCSV(reportStudents);
                            Log.Info("Saving CSV file (" + fileName + ")");
                            if (FileHelpers.FileExists(fileName))
                            {
                                FileHelpers.DeleteFile(fileName);
                            }
                            FileHelpers.SaveFile(csvContents, fileName);
                            Log.Info("Done!");
                            
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                        }
                    }
                    else
                    {
                        Log.Error("Configuration file not found");
                        Log.Info("Creating new config file (" + Config.configFileName + ")...");
                        Log.Info("Please edit the file and try again");
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
