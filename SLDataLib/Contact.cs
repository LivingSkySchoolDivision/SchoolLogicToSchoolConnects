using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SLDataLib
{
    public class Contact
    {
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Relation { get; set; }
        public int Priority { get; set; }
        public string Telephone_Home { get; set; }
        public string Telephone_Work { get; set; }
        public string Telephone_Cell { get; set; }
        public string Email { get; set; }

        public Contact() { }
        
        public static List<Contact> LoadForStudent(SqlConnection connection, string StudentNumber)
        {

            List<Contact> studentContacts = new List<Contact>();

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
                    studentContacts.Add(new Contact()
                    {
                        GivenName = dataReader["FirstName"].ToString().Trim(),
                        Surname = dataReader["LastName"].ToString().Trim(),
                        Relation = dataReader[""].ToString().Trim(),
                        Priority = Helpers.ParseInt(dataReader[""].ToString().Trim()),
                        Telephone_Cell = dataReader[""].ToString().Trim(),
                        Telephone_Home = dataReader[""].ToString().Trim(),
                        Telephone_Work = dataReader[""].ToString().Trim(),
                        Email = dataReader[""].ToString().Trim()
                    });
                }
            }

            sqlCommand.Connection.Close();

            return studentContacts;
        }

    }
}
