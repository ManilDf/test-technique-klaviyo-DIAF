using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace KlaviyoTest.Infrastructure;

public class SqlConnectionFactory : IDbConnectionFactory
{
    public DbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);
}
