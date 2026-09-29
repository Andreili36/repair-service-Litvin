using RepairService.Db;
using RepairService.Shared.Contracts;
using System;
using System.Collections.Generic;

namespace RepairService.Modules.AppsModule.Components
{
    public class SectionDescriptor
    {
        public string ObjectCode { get; set; } = "";
        public string Title { get; set; } = "";

        public Func<DbUnibridge, IEnumerable<object>> LoadData { get; set; }
            = _ => Array.Empty<object>();

        public List<ColumnDescriptor> Columns { get; set; } = new();
        public List<FormField> FormFields { get; set; } = new();

        public Action<DbUnibridge, Dictionary<string, object?>> OnCreate { get; set; }
            = (_, _) => { };

        public Action<DbUnibridge, object, Dictionary<string, object?>> OnUpdate { get; set; }
            = (_, _, _) => { };

        public Action<DbUnibridge, object> OnDelete { get; set; }
            = (_, _) => { };
    }
}