using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Easr
{
    class Persontable
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int id  {get; set;} // AutoIncrement and set primarykey  
        //public string Name {get; set;} 
        
        //public string Specialty {get; set;}
        //public string GroupMode { get; set; }
       // public string EasGroup { get; set; }
        //public string AltEasGroup { get; set; }
        public int AppSerialCode { get; set; }
        //public string AppStatus { get; set; }
    }
}

