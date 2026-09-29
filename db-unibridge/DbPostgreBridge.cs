using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace RepairService.Db
{
    public class DbPostgreBridge
    {
        private readonly DbConfig _config;

        public DbPostgreBridge(DbConfig config)
        {
            _config = config;
        }

        public void TestConnection()
        {
            using var conn = Open();
            using var cmd = new NpgsqlCommand("SELECT 1", conn);
            cmd.ExecuteScalar();
        }

        public List<T> SqlQuery<T>(
            string sql,
            Func<IDataRecord, T> mapper,
            Dictionary<string, object?>? parameters = null)
        {
            using var conn = Open();
            using var cmd = CreateCommand(conn, sql, parameters);
            using var reader = cmd.ExecuteReader();

            var list = new List<T>();
            while (reader.Read())
                list.Add(mapper(reader));
            return list;
        }

        public object? SqlScalar(
            string sql,
            Dictionary<string, object?>? parameters = null)
        {
            using var conn = Open();
            using var cmd = CreateCommand(conn, sql, parameters);
            var result = cmd.ExecuteScalar();
            return result is DBNull ? null : result;
        }

        public int SqlExecute(
            string sql,
            Dictionary<string, object?>? parameters = null)
        {
            using var conn = Open();
            using var cmd = CreateCommand(conn, sql, parameters);
            return cmd.ExecuteNonQuery();
        }

        private NpgsqlConnection Open()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = _config.Host,
                Port = _config.Port,
                Database = _config.Name,
                Username = _config.User,
                Password = _config.Password
            };
            var conn = new NpgsqlConnection(builder.ConnectionString);
            conn.Open();
            return conn;
        }

        private static NpgsqlCommand CreateCommand(
            NpgsqlConnection conn, string sql, Dictionary<string, object?>? parameters)
        {
            var cmd = new NpgsqlCommand(sql, conn);
            if (parameters != null)
                foreach (var (key, value) in parameters)
                    cmd.Parameters.AddWithValue(key, value ?? DBNull.Value);
            return cmd;
        }
    }
}