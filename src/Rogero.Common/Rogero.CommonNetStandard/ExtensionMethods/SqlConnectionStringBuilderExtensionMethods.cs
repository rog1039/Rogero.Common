using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogero.Common.ExtensionMethods
{
    public static class SqlConnectionStringBuilderExtensionMethods
    {
        public static SqlConnection ToSqlConnection(this SqlConnectionStringBuilder builder)
        {
            return new SqlConnection(builder.ConnectionString);
        }
    }
}
