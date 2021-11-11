using Microsoft.Data.SqlClient;

namespace Rogero.Common.ExtensionMethods;

public static class SqlConnectionStringBuilderExtensionMethods
{
    public static SqlConnection ToSqlConnection(this SqlConnectionStringBuilder builder)
    {
        return new SqlConnection(builder.ConnectionString);
    }
}