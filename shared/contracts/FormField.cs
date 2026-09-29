using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairService.Shared.Contracts
{
    public class FormField
    {
        public string Key { get; set; } = "";
        public string Label { get; set; } = "";
        public object? Value { get; set; }
        public FieldKind Kind { get; set; } = FieldKind.Text;
        public bool IsMandatory { get; set; } = false;
        public int ColumnSpan { get; set; } = 1;
        public Func<Task<List<OptionItem>>>? OptionsLoader { get; set; }
    }
}