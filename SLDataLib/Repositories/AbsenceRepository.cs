using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLDataLib
{
    public class AbsenceRepository
    {
        private Dictionary<int, List<Absence>> _cache;

        private Dictionary<int, List<Absence>> GetCache(SqlConnection connection, DateTime startDate, DateTime endDate)
        {
            if (_cache == null)
            {
                _cache = new Dictionary<int, List<Absence>>();

                SqlCommand sqlCommand = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText =
                        "SELECT Attendance.iStudentID, Attendance.dDate, Attendance.iBlockNumber, Attendance.iMinutes, Attendance.iSchoolID, AttendanceReasons.lExcusable, Attendance.iAttendanceReasonsID as reasonID, AttendanceStatus.cName AS AttendanceStatus FROM Attendance LEFT OUTER JOIN AttendanceStatus ON Attendance.iAttendanceStatusID = AttendanceStatus.iAttendanceStatusID LEFT OUTER JOIN AttendanceReasons ON Attendance.iAttendanceReasonsID = AttendanceReasons.iAttendanceReasonsID" +
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

                        // Figure out if this absence is excused
                        // As of September 2018, the ONLY "unexcused" absences are:
                        //  - No reason
                        //  - Absence: Known reason
                        //  - Absence: Medical
                        // Everything else is not considered an absence

                        int reasonID = Helpers.ParseInt(dataReader["reasonID"].ToString().Trim());
                        bool isExplained = reasonID != 0;
                        bool isExcused = true;
                        if (isExplained == false)
                        {
                            isExcused = false;
                        }
                        // Known Reason and Medical are reason IDs 98 and 100
                        if ((reasonID == 98) || (reasonID == 100))
                        {
                            isExcused = false;
                        }

                        _cache[studentID].Add(new Absence()
                        {
                            BlockNumber = Helpers.ParseInt(dataReader["iBlockNumber"].ToString().Trim()),
                            Date = Helpers.ParseDate(dataReader["dDate"].ToString().Trim()),
                            IsExcused = isExcused,
                            IsExplained = isExplained,
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
        
        public List<Absence> LoadAbsencesFor(SqlConnection connection, School school, Student student, DateTime startDate,
            DateTime endDate)
        {
            if (GetCache(connection, startDate, endDate).ContainsKey(student.DatabaseID))
            {
                return GetCache(connection, startDate, endDate)[student.DatabaseID].Where(a => !a.IsExcused && a.IsAbsence && a.SchoolDatabaseID == school.DatabaseID).ToList();
            }
            else
            {
                return new List<Absence>();
            }
        }

    }
}
