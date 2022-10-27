using System;
namespace Loader.MapperModels.TabularMapperModels
{
    public class TabularPropertyMapper
    {
        public string? PropertyName { get; set; }
        public string? PropertyValue { get; set; }
        public string? PropertyValueUnit { get; set; }
        public int PropertyOrder { get; set; }

        public TabularPropertyMapper()
        {
        }
    }
}

