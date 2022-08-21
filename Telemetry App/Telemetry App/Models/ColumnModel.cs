using System;
using System.Collections.Generic;
using System.Text;

namespace Telemetry_App.Models
{
    class ColumnModel
    {
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
#nullable enable
        public string? Default { get; set; } = null;
#nullable disable
        public string SerialDataIDBinding { get; set; }
    }
}
