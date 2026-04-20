using CaseItau.Application.Abstractions.Data;
using Npgsql;
using System.Data;

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
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        return connection;
    }
}
