using System;
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
        public bool IsTrackDaily { get; set; }
        
        public Student() { }
    
    }
}
