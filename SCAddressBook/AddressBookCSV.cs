﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLDataLib;

namespace SCAddressBook
{
    public static class AddressBookCSV
    {
        public static MemoryStream GenerateCSV(List<Student> students)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            // Headings
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("\"SchoolID\"," +
                               "\"StudentID\"," +
                               "\"StudentLastName\"," +
                               "\"StudentFirstName\"," +
                               "\"Grade\"," +
                               "\"HomeRoom\"," +
                               "\"ContactLastName\"," +
                               "\"ContactFirstName\"," +
                               "\"ContactRelation\"," +
                               "\"ContactPriority\"," +
                               "\"ContactHomePhone\"," +
                               "\"ContactMobilePhone\"," +
                               "\"ContactWorkPhone\"," +
                               "\"ContactEmail\"");
            writer.WriteLine(headingLine.ToString());

            // Data
            StringBuilder studentLine = new StringBuilder();
            foreach (Student student in students)
            {
                foreach (Contact contact in student.Contacts)
                {
                    studentLine.Clear();
                    studentLine.Append("\"" + student.SchoolGovID + "\"" + ",");
                    studentLine.Append("\"" + student.StudentNumber + "\"" + ",");
                    studentLine.Append("\"" + student.Surname + "\"" + ",");
                    studentLine.Append("\"" + student.GivenName + "\"" + ",");
                    studentLine.Append("\"" + student.Grade + "\"" + ",");
                    studentLine.Append("\"" + student.HomeRoom + "\"" + ",");
                    studentLine.Append("\"" + contact.Surname + "\"" + ",");
                    studentLine.Append("\"" + contact.GivenName + "\"" + ",");
                    studentLine.Append("\"" + contact.Relation + "\"" + ",");
                    studentLine.Append("\"" + contact.Priority + "\"" + ",");
                    studentLine.Append("\"" + Helpers.FormatTelephoneNumber_JustNumbers(contact.Telephone_Home) + "\"" + ",");
                    studentLine.Append("\"" + Helpers.FormatTelephoneNumber_JustNumbers(contact.Telephone_Cell) + "\"" + ",");
                    studentLine.Append("\"" + string.Empty + "\"" + ",");
                    studentLine.Append("\"" + contact.Email + "\"" + "");
                    writer.WriteLine(studentLine.ToString());                    
                }
            }

            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }

    }
}
