using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLDataLib.Repositories
{
    public class StudentRepository
    {
        public List<Student> LoadForSchool(SqlConnection connection, School school, DateTime effectiveDate)
        {
            List<Student> schoolStudents = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = "SELECT Student.cStudentNumber, Student.cFirstName, Student.cLastName, Grades.cName AS Grade, Homeroom.cName AS Homeroom, School.cCode AS SchoolGovID, StudentStatus.iStudentID, StudentStatus.dInDate, StudentStatus.dOutDate, Student.iTrackID, Student.iSchoolID, Location.cPhone AS StudentPhoneNumber, Track.lDaily FROM Homeroom RIGHT OUTER JOIN Grades RIGHT OUTER JOIN Location RIGHT OUTER JOIN Student LEFT OUTER JOIN Track ON Student.iTrackID = Track.iTrackID ON Location.iLocationID = Student.iLocationID ON Grades.iGradesID = Student.iGradesID ON Homeroom.iHomeroomID = Student.iHomeroomID RIGHT OUTER JOIN StudentStatus ON Student.iStudentID = StudentStatus.iStudentID LEFT OUTER JOIN School ON StudentStatus.iSchoolID = dbo.School.iSchoolID WHERE (dbo.School.cCode = @SCHOOLID) AND (dbo.StudentStatus.dInDate <= @ENDDATE) AND (dbo.StudentStatus.dOutDate = @NULLDATE OR dbo.StudentStatus.dOutDate >= @STARTDATE) AND (dbo.Student.iTrackID <> 0) ORDER BY dbo.Student.cLastName, dbo.Student.cFirstName"
            };
            sqlCommand.Parameters.AddWithValue("SCHOOLID", school.GovernmentID);
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
                        Grade = Helpers.FormatGrade(dataReader["Grade"].ToString().Trim()),
                        HomeRoom = dataReader["HomeRoom"].ToString().Trim(),
                        Contacts = new List<Contact>(),
                        SchoolGovID = dataReader["SchoolGovID"].ToString().Trim(),
                        SchoolDatabaseID = Helpers.ParseInt(dataReader["iSchoolID"].ToString().Trim()),
                        TelephoneNumber = dataReader["StudentPhoneNumber"].ToString().Trim(),
                        IsTrackDaily = Helpers.ParseBool(dataReader["lDaily"].ToString().Trim())
                    });
                }
            }

            sqlCommand.Connection.Close();

            return schoolStudents;

        }
    }
}
