﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLDataLib;

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
            if (args.Any())
            {
                string parsedSchoolID = string.Empty;
                string fileName = string.Empty;
                string date = string.Empty;

                foreach (string argument in args)
                {
                    if (argument.ToLower().StartsWith("/schoolid:"))
                    {
                        parsedSchoolID = argument.Substring(10,argument.Length - 10);
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
                        DateTime parsedDate = Helpers.ParseDate(date);

                        try
                        {
                            Logging.ToLog("----------------------------------------------------------------");
                            Logging.Info("Creating address book file for school " + parsedSchoolID + " for date " + parsedDate.ToLongDateString());

                            List<Student> schoolStudents = new List<Student>();
                            using (SqlConnection connection = new SqlConnection(Config.dbConnectionString_SchoolLogic))
                            {
                                Logging.Info("Loading students");
                                schoolStudents = Student.LoadForSchool(connection, parsedSchoolID,
                                    parsedDate);

                                Logging.Info("Loaded " + schoolStudents.Count + " students for school " + parsedSchoolID);

                                // Load student contacts
                                Logging.Info("Loading student contacts");
                                foreach (Student student in schoolStudents)
                                {
                                    student.Contacts = Contact.LoadForStudent(connection, student.DatabaseID);
                                }
                            }

                            
                            Logging.Info("Creating CSV data");
                            MemoryStream csvContents = AddressBookCSV.GenerateCSV(schoolStudents);
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
