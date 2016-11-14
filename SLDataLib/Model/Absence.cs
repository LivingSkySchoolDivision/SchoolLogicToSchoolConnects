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

        



    }
}
