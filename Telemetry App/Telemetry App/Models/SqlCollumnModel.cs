using System;
using System.Collections.Generic;
using System.Text;

namespace Telemetry_App.Models
{
    class SqlCollumnModel
    {
        public string collumnName = string.Empty;
        public string dataType = string.Empty;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public string? defaultData = null;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}
