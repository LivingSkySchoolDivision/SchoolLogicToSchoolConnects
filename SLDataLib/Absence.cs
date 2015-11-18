using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLDataLib
{
    public class Absence
    {
        public int StudentDatabaseID { get; set; }
        public DateTime Date { get; set; }
        public int BlockNumber { get; set; }
        public int LateMinutes { get; set; }
        public bool IsExcused { get; set; }
        public int SchoolDatabaseID { get; set; }
        public string Status { get; set; }

        public Absence()
        {
            
        }

        public bool IsAbsence
        {
            get
            {
                return this.Status.ToLower() == "absent";
            }
        }

        public bool IsLate
        {
            get {
                return this.Status.ToLower() == "late";
            }
        }

        public bool IsLeaveEarly
        {
            get
            {
                return this.Status.ToLower() == "leave early";
            }
        }

        private static Dictionary<int, List<Absence>> _cache;

        private static Dictionary<int, List<Absence>> GetCache(SqlConnection connection, DateTime startDate, DateTime endDate)
        {
            if (_cache == null)
            {
                _cache = new Dictionary<int, List<Absence>>();

                SqlCommand sqlCommand = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText =
                        "SELECT Attendance.iStudentID, Attendance.dDate, Attendance.iBlockNumber, Attendance.iMinutes, Attendance.iSchoolID, AttendanceReasons.lExcusable, AttendanceStatus.cName AS AttendanceStatus FROM Attendance LEFT OUTER JOIN AttendanceStatus ON Attendance.iAttendanceStatusID = AttendanceStatus.iAttendanceStatusID LEFT OUTER JOIN AttendanceReasons ON Attendance.iAttendanceReasonsID = AttendanceReasons.iAttendanceReasonsID" +
                        " WHERE (Attendance.dDate <= @ENDDATE) AND (Attendance.dDate >= @STARTDATE) ORDER BY Attendance.iStudentID ASC, Attendance.iBlockNumber ASC"
                };
                // (Attendance.iSchoolID = @SCHOOLID) AND
                //sqlCommand.Parameters.AddWithValue("SCHOOLID", schoolDatabaseID);
                sqlCommand.Parameters.AddWithValue("STARTDATE", startDate);
                sqlCommand.Parameters.AddWithValue("ENDDATE", endDate);

                sqlCommand.Connection.Open();

                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        int studentID = Helpers.ParseInt(dataReader["iStudentID"].ToString().Trim());
                        
                        if (!_cache.ContainsKey(studentID))
                        {
                            _cache.Add(studentID, new List<Absence>());
                        }

                        _cache[studentID].Add(new Absence()
                        {
                            BlockNumber = Helpers.ParseInt(dataReader["iBlockNumber"].ToString().Trim()),
                            Date = Helpers.ParseDate(dataReader["dDate"].ToString().Trim()),
                            IsExcused = Helpers.ParseBool(dataReader["lExcusable"].ToString().Trim()),
                            LateMinutes = Helpers.ParseInt(dataReader["iMinutes"].ToString().Trim()),
                            SchoolDatabaseID = Helpers.ParseInt(dataReader["iSchoolID"].ToString().Trim()),
                            StudentDatabaseID = Helpers.ParseInt(dataReader["iStudentID"].ToString().Trim()),
                            Status = dataReader["AttendanceStatus"].ToString().Trim()
                        });
                    }
                }

                sqlCommand.Connection.Close();
            }

            return _cache;

        }


        public static List<Absence> LoadAbsencesFor(SqlConnection connection, Student student, DateTime startDate,
            DateTime endDate)
        {
            if (GetCache(connection, startDate, endDate).ContainsKey(student.DatabaseID))
            {
                return GetCache(connection, startDate, endDate)[student.DatabaseID].Where(a => !a.IsExcused && a.IsAbsence).ToList();
            }
            else
            {
                return new List<Absence>();
            }
        }


    }
}
