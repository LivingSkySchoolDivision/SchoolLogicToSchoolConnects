﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security;

namespace SLDataLib
{
    public class Student
    {
        public string SchoolGovID { get; set; }
        public int SchoolDatabaseID { get; set; }
        public int DatabaseID { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string StudentNumber { get; set; }
        public string Grade { get; set; }
        public string HomeRoom { get; set; }
        public string TelephoneNumber { get; set; }
        public List<Contact> Contacts { get; set; }
        
        public Student() { }

        public static List<Student> LoadForSchool(SqlConnection connection, string SchoolGovID, DateTime effectiveDate)
        {
            List<Student> schoolStudents = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = "SELECT Student.cStudentNumber, Student.cFirstName, Student.cLastName, Grades.cName AS Grade, Homeroom.cName AS Homeroom, School.cCode AS SchoolGovID, StudentStatus.iStudentID, StudentStatus.dInDate, StudentStatus.dOutDate, Student.iTrackID, Student.iSchoolID, Location.cPhone AS StudentPhoneNumber FROM StudentStatus LEFT OUTER JOIN Homeroom RIGHT OUTER JOIN Grades RIGHT OUTER JOIN Student LEFT OUTER JOIN Location ON Student.iLocationID = Location.iLocationID ON Grades.iGradesID = Student.iGradesID ON Homeroom.iHomeroomID = Student.iHomeroomID ON StudentStatus.iStudentID = Student.iStudentID LEFT OUTER JOIN School ON StudentStatus.iSchoolID = School.iSchoolID" +
                              " WHERE (dbo.School.cCode = @SCHOOLID) AND (dbo.StudentStatus.dInDate <= @ENDDATE) AND (dbo.StudentStatus.dOutDate = @NULLDATE OR dbo.StudentStatus.dOutDate >= @STARTDATE) AND (dbo.Student.iTrackID <> 0) ORDER BY dbo.Student.cLastName, dbo.Student.cFirstName"
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
                        Grade = Helpers.FormatGrade(dataReader["Grade"].ToString().Trim()),
                        HomeRoom = dataReader["HomeRoom"].ToString().Trim(),
                        Contacts =  new List<Contact>(),
                        SchoolGovID = dataReader["SchoolGovID"].ToString().Trim(),
                        SchoolDatabaseID = Helpers.ParseInt(dataReader["iSchoolID"].ToString().Trim()),
                        TelephoneNumber = dataReader["StudentPhoneNumber"].ToString().Trim()
                    });
                }
            }

            sqlCommand.Connection.Close();
            
            return schoolStudents;

        }
    
    }
}
