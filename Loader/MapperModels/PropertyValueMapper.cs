using System;
namespace PlatformTM.Models
{
    public class PropertyValueMapper
    {
        public string? SourceFileName { get; internal set; }
        public string? SourceVariableId { get; internal set; }
        //public string? SourceVariableName { get; internal set; }
        //public string? SourceVariableText { get; internal set; }
        public ValueExpression ValueExpression { get; set; }

        public PropertyValueMapper(string valueString)
        {
            ValueExpression = new(valueString);
           // ValueExpression.ValueString = valueString;
           // ValueExpression.ProcessValueExpression();
        }

        
    }
}

