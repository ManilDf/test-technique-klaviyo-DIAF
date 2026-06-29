using System.Data.Common;

namespace KlaviyoTest.Infrastructure;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection(string connectionString);
}
