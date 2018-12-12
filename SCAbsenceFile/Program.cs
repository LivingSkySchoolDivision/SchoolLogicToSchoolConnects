using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLDataLib;
using SLDataLib.Repositories;

namespace SCAbsenceFile
{
    class Program
    {
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
            Console.WriteLine(" /allschools");
            Console.WriteLine(" Select all schools (ignoring //schoolid)");
            Console.WriteLine("");
            Console.WriteLine(" /filename:filename.csv");
            Console.WriteLine(" Specify the filename");
            Console.WriteLine("");
            Console.WriteLine(""); 
            Console.WriteLine(" /day:[today|yesterday|now]");
            Console.WriteLine(" Which day would you like absences from.");
            Console.WriteLine("");
            Console.WriteLine(" OPTIONAL:");
            Console.WriteLine("");
            Console.WriteLine(" /JustPeriodAttendance");
            Console.WriteLine(" Only include students who are in tracks using period attendance.");
            Console.WriteLine("");
            Console.WriteLine(" /JustDailyAttendance");
            Console.WriteLine(" Only include students who are in tracks using daily (AM/PM) attendance.");
            Console.WriteLine("");
            Console.WriteLine(" /grades:pk,k,1,2,3,4,5,6,7,8,9,10,11,12");
            Console.WriteLine("");
            Console.WriteLine(" /blocks:1,2,3,4,5,6,7,8");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("scabsencefile.exe /allschools /filename:LSKYSDAbsencePeriod.csv /date:today /JustPeriodAttendance");
            Console.WriteLine("");
            Console.WriteLine("scabsencefile.exe /allschools /filename:LSKYSDAbsenceDaily.csv /date:today /JustDailyAttendance");
            Console.WriteLine("");
            Console.WriteLine("scabsencefile.exe /schoolid:4410413 /filename:MacklinAbsenceHS.csv /date:today /grades:7,8,9,10,11,12");
            Console.WriteLine("");
            Console.WriteLine("scabsencefile.exe /schoolid:4410413 /filename:MacklinAbsenceElem.csv /date:today /grades:pk,k,1,2,3,4,5,6");
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {            
            if (args.Any())
            {
                string fileName = string.Empty;
                string date = string.Empty;
                List<string> grades = new List<string>() { "pk", "k", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
                // There aren't nearly this many blocks in a day, but this future proofs it. It can't pull absences for blocks that dont exist
                List<int> blocks = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }; 
                bool allSchools = false;
                bool onlyPeriodAttendance = false;
                bool onlyDailyAttendance = false;

                List<string> selectedSchoolIDs = new List<string>();

                foreach (string argument in args)
                {
                    if (argument.ToLower().StartsWith("/schoolid:"))
                    {
                        foreach (string enteredID in argument.Substring(10, argument.Length - 10).Split(new char[] {';', ','}))
                        {
                            if (!string.IsNullOrEmpty(enteredID))
                            {
                                selectedSchoolIDs.Add(enteredID);
                            }
                        }
                    }
                    else if (argument.ToLower().StartsWith("/filename:"))
                    {
                        fileName = argument.Substring(10, argument.Length - 10);
                    }
                    else if (argument.ToLower().StartsWith("/allschools"))
                    {
                        allSchools = true;
                    }
                    else if (argument.ToLower().StartsWith("/justperiodattendance"))
                    {
                        onlyPeriodAttendance = true;
                    }
                    else if (argument.ToLower().StartsWith("/justdailyattendance"))
                    {
                        onlyDailyAttendance = true;
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
                    else if (argument.ToLower().StartsWith("/blocks:"))
                    {
                        string blob = argument.Substring(8, argument.Length - 8);
                        char[] splitChars = { ',', ';' };
                        List<string> unparsedBlocks = blob.Split(splitChars).ToList();
                        blocks.Clear();
                        foreach (string s in unparsedBlocks)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                int parsed = 0;
                                if (int.TryParse(s, out parsed))
                                {
                                    if (parsed > 0)
                                    {
                                        blocks.Add(parsed);
                                    }
                                }
                            }
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
                            Logging.ToLog("----------------------------------------------------------------");
                            Logging.Info(" Creating absence file for date " + parsedDate.ToLongDateString());
                            Logging.Info(" File creation started: " + DateTime.Now);

                            Dictionary<Student, List<Absence>> studentsWithAbsences = new Dictionary<Student, List<Absence>>();
                            List<School> selectedSchools = new List<School>();
                            AbsenceRepository absenceRepo = new AbsenceRepository();
                            StudentRepository studentRepo = new StudentRepository();
                            using (
                                SqlConnection connection = new SqlConnection(Config.dbConnectionString_SchoolLogic))
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
                                
                                Logging.Info("Loading students");
                                foreach (School school in selectedSchools)
                                {
                                    List<Student> schoolStudents = studentRepo.LoadForSchool(connection, school, parsedDate).Where(s => grades.Contains(s.Grade.ToLower())).ToList();

                                    if (onlyDailyAttendance && !onlyPeriodAttendance)
                                    {
                                        Logging.Info("Only using daily attendance students");
                                        schoolStudents = schoolStudents.Where(s => s.IsTrackDaily == true).ToList();
                                    }

                                    if (onlyPeriodAttendance && !onlyDailyAttendance)
                                    {
                                        Logging.Info("Only using period attendance students");
                                        schoolStudents = schoolStudents.Where(s => s.IsTrackDaily == false).ToList();
                                    }

                                    Logging.Info("Loaded " + schoolStudents.Count + " students for school " + school.Name);

                                    int schoolAbsenceCount = 0;

                                    // Load student absences
                                    foreach (Student student in schoolStudents)
                                    {
                                        List<Absence> studentAbsences = absenceRepo.LoadAbsencesFor(connection, school, student,
                                            parsedDate, parsedDate.AddHours(23.5)).Where(abs => blocks.Contains(abs.BlockNumber)).ToList();
                                        if (studentAbsences.Count > 0)
                                        {
                                            studentsWithAbsences.Add(student, studentAbsences);
                                        }
                                        schoolAbsenceCount += studentAbsences.Count;
                                    }
                                    Logging.Info(" Loaded " + schoolAbsenceCount + " absences for school " + school.Name);
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
