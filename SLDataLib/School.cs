using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLDataLib
{
    public class School
    {
        public static List<string> LoadSchoolIDNumbers(SqlConnection connection)
        {
            List<string> schoolIDs = new List<string>();

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
                    schoolIDs.Add(dataReader["cCode"].ToString().Trim());
                }
            }
            sqlCommand.Connection.Close();
            return schoolIDs;
        } 
    }
}
