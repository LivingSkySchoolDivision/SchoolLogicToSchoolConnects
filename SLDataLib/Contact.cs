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
        public bool LivesWithStudent { get; set; }
        public Contact() { }
        
        public static List<Contact> LoadForStudent(SqlConnection connection, int iStudentID)
        {

            List<Contact> studentContacts = new List<Contact>();

            SqlCommand sqlCommand = new SqlCommand()
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = "SELECT dbo.ContactRelation.iStudentID, dbo.LookupValues.cName AS Relation, dbo.ContactRelation.iContactPriority, dbo.Contact.cLastName, dbo.Contact.cFirstName, dbo.Contact.cBusPhone, dbo.Contact.mEmail, dbo.Contact.mCellphone, dbo.Location.cPhone FROM dbo.LookupValues RIGHT OUTER JOIN dbo.ContactRelation ON dbo.LookupValues.iLookupValuesID = dbo.ContactRelation.iLV_RelationID LEFT OUTER JOIN dbo.Contact ON dbo.ContactRelation.iContactID = dbo.Contact.iContactID LEFT OUTER JOIN dbo.Location ON dbo.Contact.iLocationID = dbo.Location.iLocationID WHERE iStudentID=@ISTUDENTID"
            };
            sqlCommand.Parameters.AddWithValue("ISTUDENTID", iStudentID);

            sqlCommand.Connection.Open();

            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    studentContacts.Add(new Contact()
                    {
                        GivenName = dataReader["cFirstName"].ToString().Trim(),
                        Surname = dataReader["cLastName"].ToString().Trim(),
                        Relation = dataReader["Relation"].ToString().Trim(),
                        Priority = Helpers.ParseInt(dataReader["iContactPriority"].ToString().Trim()),
                        Telephone_Cell = dataReader["mCellPhone"].ToString().Trim(),
                        Telephone_Home = dataReader["cPhone"].ToString().Trim(),
                        Telephone_Work = dataReader["cBusPhone"].ToString().Trim(),
                        Email = dataReader["mEmail"].ToString().Trim(),
                        LivesWithStudent = Helpers.ParseBool(dataReader["mEmail"].ToString().Trim())
                    });
                }
            }

            sqlCommand.Connection.Close();

            return studentContacts;
        }

    }
}
