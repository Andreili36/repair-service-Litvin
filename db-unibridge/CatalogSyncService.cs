using System.Collections.Generic;

namespace RepairService.Db
{
    public class CatalogSyncService
    {
        private readonly DbUnibridge _db;

        public CatalogSyncService(DbUnibridge db)
        {
            _db = db;
        }

        public void Sync(IEnumerable<CatalogDefinition> catalogs)
        {
            foreach (var catalog in catalogs)
            {
                if (!TableExists(catalog.ObjectCode))
                    _db.SqlExecute(catalog.CreateTableSql);

                if (!string.IsNullOrWhiteSpace(catalog.CreateIndexesSql))
                    _db.SqlExecute(catalog.CreateIndexesSql);
            }
        }

        private bool TableExists(string tableName)
        {
            var result = _db.SqlScalar(
                @"SELECT EXISTS (
                    SELECT 1 FROM information_schema.tables
                    WHERE table_schema = 'public' AND table_name = @t
                  )",
                new Dictionary<string, object?> { ["@t"] = tableName });

            return result is true;
        }
    }
}