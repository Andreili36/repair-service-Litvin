namespace RepairService.Db
{
    public class CatalogDefinition
    {
        public string ObjectCode { get; init; } = "";
        public string CreateTableSql { get; init; } = "";
        public string? CreateIndexesSql { get; init; }
    }
}