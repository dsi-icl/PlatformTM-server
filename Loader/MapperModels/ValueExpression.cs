using System;
namespace PlatformTM.Models
{
    public class ValueExpression
    {
        public ValueExpression(string expString)
        {
            ValueString = expString;
            ProcessValueExpression();
        }

        public ValueExpression()
        {

        }

        public Dictionary<string, string> DataDictionary { get; set; }
        public bool IsCoded { get; set; }
        public bool IsSrcValue { get; set; }
        public bool IsRef { get; set; }
        public bool HasPlaceholderRef { get; set; }
        public string ValueString { get; set; }
        public string Function { get; internal set; }
        public string[] SourceVariables { get; internal set; }
        public string TruthyValue { get; internal set; }
        public string FalsyValue { get; internal set; }
        public string ConditionValue { get; internal set; }
        public bool IsValueString { get; private set; }
        public string PlaceholderString { get; internal set; }

        public void ProcessValueExpression()
        {
            if (ValueString.StartsWith("IF ANY("))
            {
                int startIndex = ValueString.IndexOf('(');
                int endIndex = ValueString.IndexOf(')');
                var sourceVariables_str = ValueString.Substring(startIndex + 1, endIndex - startIndex - 1);
                var variables = sourceVariables_str.Split(',');


                int thenIndex = ValueString.IndexOf("THEN");
                int elseIndex = ValueString.IndexOf("ELSE");

                var truthyValue = ValueString.Substring(thenIndex + 4, elseIndex - thenIndex - 4).Replace('\"', ' ').Trim();
                var falsyValue = ValueString.Substring(elseIndex + 4).Replace('\"', ' ').Trim();

                int condIndex = ValueString.IndexOf("==");
                var condValue = ValueString.Substring(condIndex + 2, thenIndex - condIndex - 2).Replace('\"', ' ').Trim();

                Function = "ANY";
                SourceVariables = variables;
                TruthyValue = truthyValue;
                FalsyValue = falsyValue;
                ConditionValue = condValue;
            }

            else if (ValueString.Contains('|') && ValueString.Contains('='))
            {
                DataDictionary = new();

                int elseIndex;
                var tempVE = ValueString;
                if (ValueString.Contains("ELSE $VAL"))
                {
                    elseIndex = ValueString.IndexOf("ELSE");
                    tempVE = ValueString.Substring(0, elseIndex - 1);
                    DataDictionary.Add("$VAL", "$VAL");
                }




                string[] keyValues = tempVE.Split("|");
                foreach (string kvpair in keyValues)
                {
                    string[] kv = kvpair.Split("=");
                    if (kv.Length > 1)
                    {
                        DataDictionary.Add(kv[0].ToUpper().Replace("\"", ""), kv[1].Replace("\"",""));
                        IsCoded = true;
                    }

                }
            }

            else if (ValueString == "$VAL")
            {
                IsSrcValue = true;
            }
            else if (ValueString.StartsWith('$'))
            {
                var srcVariable = ValueString[1..];
                if (srcVariable != null)
                {
                    SourceVariables = new string[] { srcVariable };
                    IsRef = true;
                }   
            }
            else if (ValueString.Contains('$'))
            {
                PlaceholderString = ValueString.Split(" ").ToList().Find(s=>s.StartsWith('$'));

                SourceVariables = new string[] { PlaceholderString?.Replace("$","") };

                HasPlaceholderRef = true;
            }

            else
                IsValueString = true;
        }

        public string EvaluateExpression(string srcValue)
        {
            if (IsCoded)
            {
                if (DataDictionary.ContainsKey("$ANY") && srcValue != "")
                    return DataDictionary["$ANY"];

                int num;
                if(DataDictionary.ContainsKey("$Z+") && Int32.TryParse(srcValue, out num) && num >0)
                    return DataDictionary["$Z+"];



                if (DataDictionary.TryGetValue(srcValue.ToUpper(), out string pv))
                {
                    if (pv.ToUpper() == "$SKIP")
                        return "";
                    else
                        return pv;
                }
                return pv;
                //else if (pvMapper.DataDictionary.ContainsKey("$VAL"))
                //    obs.PropertyValues.Add(dataValue);

            }
            else if (IsSrcValue)
            {
                return srcValue;
            }
            else if (IsValueString)
            {
                return ValueString;
            }
            else
                return null;
        }

        public string? EvaluateExpression(Dictionary<string,string> dataRecord)
        {
            if (IsRef)
                return dataRecord[SourceVariables[0]];

            else if (HasPlaceholderRef)
            {
                var val = dataRecord[SourceVariables[0]];
                return val != "" ? ValueString.Replace(PlaceholderString, val) : "";
            }
            
            else if (IsValueString)
                return ValueString;

            else return null;
        }
    }
}

