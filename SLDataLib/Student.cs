using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SLDataLib
{
    public class Student
    {
        public int DatabaseID { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string StudentNumber { get; set; }
        public string Grade { get; set; }
        public string HomeRoom { get; set; }
        public List<Contact> Contacts { get; set; }
        
        public Student() { }

        public static List<Student> LoadForSchool(SqlConnection connection, string SchoolGovID, DateTime effectiveDate)
        {
            List<Student> schoolStudents = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = "SQL QUERY GOES HERE"
            };

            sqlCommand.Connection.Open();

            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    schoolStudents.Add(new Student()
                    {
                        GivenName = dataReader["FirstName"].ToString().Trim(),
                        Surname = dataReader["LastName"].ToString().Trim(),
                        StudentNumber = dataReader["StudentNumber"].ToString().Trim(),
                        DatabaseID = Helpers.ParseInt(dataReader["iStudentID"].ToString().Trim()),
                        Grade = dataReader["Grade"].ToString().Trim(),
                        HomeRoom = dataReader["HomeRoom"].ToString().Trim(),
                        Contacts =  new List<Contact>()
                    });
                }
            }

            sqlCommand.Connection.Close();

            // Load student contacts

            foreach (Student student in schoolStudents)
            {
                student.Contacts = Contact.LoadForStudent(connection, student.StudentNumber);
            }

            return schoolStudents;

        }
    
    }
}
