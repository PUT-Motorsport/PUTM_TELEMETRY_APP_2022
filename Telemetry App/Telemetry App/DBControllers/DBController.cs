using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Telemetry_App.DBControllers
{
    class DBController
    {
        public string Server { get; set; }
        public string DataBase { get; set; }

        public DBController()
        {
        }
    }
}
