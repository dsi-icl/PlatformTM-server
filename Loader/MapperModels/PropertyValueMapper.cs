using System;
namespace PlatformTM.Models
{
    public class PropertyValueMapper
    {
        public Dictionary<string, string>? DataDictionary { get; set; }

        public ValueExpression ValueExpression { get; set; }

        public string? Unit { get; set; }


        public string? SourceFileName { get; internal set; }
        public string? SourceVariableId { get; internal set; }
        public string? SourceVariableName { get; internal set; }
        public string? SourceVariableText { get; internal set; }

        public PropertyValueMapper(string valueString)
        {
            ValueExpression = new();
            ValueExpression.ValueString = valueString;
            ValueExpression.ProcessValueExpression();
        }

        
    }
}

