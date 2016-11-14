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
        

    }
}
