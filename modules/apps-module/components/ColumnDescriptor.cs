namespace RepairService.Modules.AppsModule.Components
{
    public enum ColumnType
    {
        Text,
        Number,
        Bool,
        Date
    }

    public class ColumnDescriptor
    {
        public string Key { get; set; } = "";
        public string Header { get; set; } = "";
        public ColumnType Type { get; set; } = ColumnType.Text;
    }
}