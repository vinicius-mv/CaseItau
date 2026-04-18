using CaseItau.Application.Abstractions.Data;
using System.Data;
using System.Data.SQLite;

namespace CaseItau.Infra.Data;

internal sealed class SqliteConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        return connection;
    }
}
