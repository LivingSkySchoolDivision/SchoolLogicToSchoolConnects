using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLDataLib
{
    public class SchoolRepository
    {
        private Dictionary<string, School> _cache { get; set; }

        public SchoolRepository(SqlConnection connection)
        {
            _cache = new Dictionary<string, School>();

            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM SCHOOL WHERE iDistrictID=1 AND cCode IS NOT NULL;"
            };

            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    if (!string.IsNullOrEmpty(dataReader["cCode"].ToString().Trim()))
                    {
                        _cache.Add(dataReader["cCode"].ToString().Trim(), new School()
                        {
                            Name = dataReader["cName"].ToString().Trim(),
                            GovernmentID = dataReader["cCode"].ToString().Trim(),
                            DatabaseID = Helpers.ParseInt(dataReader["iSchoolID"].ToString().Trim())
                        });
                    }
                }
            }
            sqlCommand.Connection.Close();
        }
        

        public School Get(int databaseID)
        {
            foreach (School school in _cache.Values)
            {
                if (school.DatabaseID == databaseID)
                {
                    return school;
                }
            }
            return null;
        }

        public School Get(string schoolNumber)
        {
            return _cache.ContainsKey(schoolNumber) ? _cache[schoolNumber] : null;
        }

        public List<School> Get(List<string> schoolGovIds)
        {
            List<School> returnMe = new List<School>();

            foreach (string id in schoolGovIds)
            {
                if (_cache.ContainsKey(id))
                {
                    returnMe.Add(_cache[id]);
                }
            }

            return returnMe;
        }
        

        public List<School> GetAll()
        {
            return _cache.Values.ToList();
        }

        public List<string> GetAllGovIDs()
        {
            return _cache.Keys.ToList();
        }


    }
    
}
