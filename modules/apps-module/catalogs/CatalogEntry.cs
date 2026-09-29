using System;
using RepairService.Modules.AppsModule.Components;

namespace RepairService.Modules.AppsModule.Catalogs
{
    public class CatalogEntry
    {
        public string ObjectCode { get; init; } = "";
        public string Title { get; init; } = "";
        public Func<SectionDescriptor> DescriptorFactory { get; init; } = () => new SectionDescriptor();
    }
}