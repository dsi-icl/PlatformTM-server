using System;
namespace PlatformTM.Models
{
    public class PropertyValueMapper
    {
        public Dictionary<string, string> DataDictionary { get; set; }

        public ValueExpression ValueExpression { get; set; }

        public string Unit { get; set; }
        //public string SourceVariableId { get; set; }
        //public string SourceVariableName { get; internal set; }
        //public string SourceVariableFileName { get; set; }

        //public bool HasDictionary { get; internal set; }
        //public bool IsExpression { get; internal set; }
        //public string Function { get; internal set; }
        //public string[] SourceVariables { get; internal set; }
        //public string TruthyValue { get; internal set; }
        //public string FalsyValue { get; internal set; }
        //public string ConditionValue { get; internal set; }

        public PropertyValueMapper(string valueString)
        {
            ValueExpression = new();
            ValueExpression.ValueString = valueString;
        }
    }
}

