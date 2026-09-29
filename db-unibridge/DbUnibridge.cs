using System;
using System.Collections.Generic;
using System.Data;

namespace RepairService.Db
{
    public class DbUnibridge
    {
        private readonly DbPostgreBridge _bridge;

        public DbUnibridge(DbConfig config)
        {
            _bridge = new DbPostgreBridge(config);
        }

        public void TestConnection() => _bridge.TestConnection();

        public List<T> SqlQuery<T>(
            string sql,
            Func<IDataRecord, T> mapper,
            Dictionary<string, object?>? parameters = null)
            => _bridge.SqlQuery(sql, mapper, parameters);

        public object? SqlScalar(
            string sql,
            Dictionary<string, object?>? parameters = null)
            => _bridge.SqlScalar(sql, parameters);

        public int SqlExecute(
            string sql,
            Dictionary<string, object?>? parameters = null)
            => _bridge.SqlExecute(sql, parameters);
    }
}