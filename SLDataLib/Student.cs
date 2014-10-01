using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SLDataLib
{
    public class Student
    {
        public string SchoolGovID { get; set; }
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
                CommandText = "SELECT dbo.Enrollment.dInDate, dbo.Enrollment.dOutDate, dbo.Enrollment.iStudentID, dbo.Student.cStudentNumber, dbo.Student.cFirstName, dbo.Student.cLastName, dbo.Grades.cName AS Grade, dbo.Homeroom.cName AS Homeroom, dbo.School.cCode AS SchoolGovID FROM dbo.School RIGHT OUTER JOIN dbo.Enrollment ON dbo.School.iSchoolID = dbo.Enrollment.iSchoolID LEFT OUTER JOIN dbo.Student LEFT OUTER JOIN dbo.Homeroom ON dbo.Student.iHomeroomID = dbo.Homeroom.iHomeroomID LEFT OUTER JOIN dbo.Grades ON dbo.Student.iGradesID = dbo.Grades.iGradesID ON dbo.Enrollment.iStudentID = dbo.Student.iStudentID WHERE SchoolGovID=@SCHOOLID"
            };
            sqlCommand.Parameters.AddWithValue("SCHOOLID", SchoolGovID);

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
                student.Contacts = Contact.LoadForStudent(connection, student.DatabaseID);
            }

            return schoolStudents;

        }
    
    }
}
