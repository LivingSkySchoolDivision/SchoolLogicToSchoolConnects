using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SLDataLib;

namespace SCAbsenceFile
{
    public static class AbsenceCSV
    {
        /// <summary>
        /// Formats a date to fit the formatting on the example report
        /// </summary>
        /// <param name="thisDate"></param>
        /// <returns></returns>
        private static string formatDate(DateTime thisDate)
        {
            return thisDate.ToString("yyyy-M-d");
        }

        public static MemoryStream GenerateCSV(Dictionary<Student, List<Absence>> studentsWithAbsences)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            // Headings
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("\"SchoolID\"," +
                               "\"StudentID\"," +
                               "\"StudentLastName\"," +
                               "\"StudentFirstName\"," +
                               "\"HomePhone\"," +
                               "\"Grade\"," +
                               "\"AbsenceDate\"," +
                               "\"PeriodsMissed\"");
            writer.WriteLine(headingLine.ToString());

            // Data
            StringBuilder studentLine = new StringBuilder();
            foreach (Student student in studentsWithAbsences.Keys)
            {
                List<Absence> unexcusedAbsences = studentsWithAbsences[student].Where(abs => abs.IsLate == false).Where(abs => abs.IsExcused == false).ToList();
                
                if (unexcusedAbsences.Count > 0)
                {
                    // Seperate this into dates - the report SHOULD only be run for a single date, but just in case it doesnt, and just in case I 
                    // modify it in the future, split different dates into different lines.

                    Dictionary<string, List<Absence>> absencesByDate = new Dictionary<string, List<Absence>>();

                    foreach (Absence abs in unexcusedAbsences)
                    {
                        if (!absencesByDate.ContainsKey(formatDate(abs.Date)))
                        {
                            absencesByDate.Add(formatDate(abs.Date), new List<Absence>());
                        }
                        absencesByDate[formatDate(abs.Date)].Add(abs);
                    }

                    foreach (string dateString in absencesByDate.Keys)
                    {
                        StringBuilder missedPeriodsString = new StringBuilder();
                        foreach (Absence abs in unexcusedAbsences)
                        {
                            missedPeriodsString.Append(abs.BlockNumber);
                        }

                        studentLine.Clear();
                        studentLine.Append("\"" + student.SchoolGovID + "\"" + ",");
                        studentLine.Append("\"" + student.StudentNumber + "\"" + ",");
                        studentLine.Append("\"" + student.Surname + "\"" + ",");
                        studentLine.Append("\"" + student.GivenName + "\"" + ",");
                        studentLine.Append("\"" + Helpers.FormatTelephoneNumber_JustNumbers(student.TelephoneNumber) + "\"" + ",");
                        studentLine.Append("\"" + student.Grade + "\"" + ",");
                        studentLine.Append("\"" + dateString + "\"" + ",");
                        studentLine.Append("\"" + missedPeriodsString.ToString() + "\"");
                        writer.WriteLine(studentLine.ToString());
                    }
                }
            }

            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }


    }
}
