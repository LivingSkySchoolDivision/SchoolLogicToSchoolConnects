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
                CommandText = "SELECT dbo.Student.cStudentNumber, dbo.Student.cFirstName, dbo.Student.cLastName, dbo.Grades.cName AS Grade, dbo.Homeroom.cName AS Homeroom, dbo.School.cCode AS SchoolGovID, dbo.StudentStatus.iStudentID, dbo.StudentStatus.dInDate, dbo.StudentStatus.dOutDate, dbo.Student.iTrackID FROM dbo.School RIGHT OUTER JOIN dbo.StudentStatus LEFT OUTER JOIN dbo.Homeroom RIGHT OUTER JOIN dbo.Student LEFT OUTER JOIN dbo.Grades ON dbo.Student.iGradesID = dbo.Grades.iGradesID ON dbo.Homeroom.iHomeroomID = dbo.Student.iHomeroomID ON dbo.StudentStatus.iStudentID = dbo.Student.iStudentID ON dbo.School.iSchoolID = dbo.StudentStatus.iSchoolID WHERE (dbo.School.cCode = @SCHOOLID) AND (dbo.StudentStatus.dInDate < @ENDDATE) AND (dbo.StudentStatus.dOutDate = @NULLDATE OR dbo.StudentStatus.dOutDate >= @STARTDATE) AND (dbo.Student.iTrackID <> 0) ORDER BY dbo.Student.cLastName, dbo.Student.cFirstName"
            };
            sqlCommand.Parameters.AddWithValue("SCHOOLID", SchoolGovID);
            sqlCommand.Parameters.AddWithValue("NULLDATE", Helpers.DatabaseNullDate);
            sqlCommand.Parameters.AddWithValue("STARTDATE", effectiveDate);
            sqlCommand.Parameters.AddWithValue("ENDDATE", effectiveDate);

            sqlCommand.Connection.Open();

            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    schoolStudents.Add(new Student()
                    {
                        GivenName = dataReader["cFirstName"].ToString().Trim(),
                        Surname = dataReader["cLastName"].ToString().Trim(),
                        StudentNumber = dataReader["cStudentNumber"].ToString().Trim(),
                        DatabaseID = Helpers.ParseInt(dataReader["iStudentID"].ToString().Trim()),
                        Grade = dataReader["Grade"].ToString().Trim(),
                        HomeRoom = dataReader["HomeRoom"].ToString().Trim(),
                        Contacts =  new List<Contact>(),
                        SchoolGovID = dataReader["SchoolGovID"].ToString().Trim()
                    });
                }
            }

            sqlCommand.Connection.Close();
            
            return schoolStudents;

        }
    
    }
}
