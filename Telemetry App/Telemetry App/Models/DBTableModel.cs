using System;
using System.Collections.Generic;
using System.Text;
using Telemetry_App.Models;

namespace Telemetry_App.Models
{
    class DBTableModel
    {
        public string TableName { get; set; }

        public int Keep { get; set; }

        public int UpdateRateMs { get; set; }

        public ColumnModel[] Columns { get; set; }
    }
}
