using System;
using System.Data;
using PlatformTM.Core.Domain.Model.Templates;

namespace Loader
{
    public class CDISCservice
    {
        public CDISCservice()
        {
        }

        internal static DatasetTemplate CreateCDISCtemplate(string cdiscTemplatesDir, string v1, string v2)
        {
            var DSinfo = TabularDataIO.ReadDataTable(Path.Combine(cdiscTemplatesDir, v1));
            var DSvarsInfo = TabularDataIO.ReadDataTable(Path.Combine(cdiscTemplatesDir, v2));

            var Template = new DatasetTemplate();
            Template.Domain = DSinfo.Rows[0]["Dataset Label"].ToString();
            Template.Class = DSinfo.Rows[0]["Class"].ToString();
            Template.Code = DSinfo.Rows[0]["Dataset Name"].ToString();
            Template.Id = DSinfo.Rows[0]["OID"].ToString();
            Template.Structure = DSinfo.Rows[0]["Structure"].ToString();
            Template.Description = DSinfo.Rows[0]["Description"].ToString();


            foreach (DataRow row in DSvarsInfo.Rows)
            {
                Template.Fields.Add(new DatasetTemplateField()
                {
                    Name = row["Variable Name"].ToString(),
                    Description = row["CDISC Notes"].ToString(),
                    DataType = row["Type"].ToString(),
                    Label = row["Variable Label"].ToString(),
                    Order = Int32.Parse(row["Variable Order"].ToString()),
                    Section = "Column",
                    AllowMultipleValues = false,
                    IsGeneric = false,
                    Id = "VS-SDTM-" + Template.Code + "-" + row["Variable Name"].ToString(),
                    UsageId = GetUsageId(row["Core"].ToString()),
                    RoleId = GetRoleId(row["Role"].ToString()),
                    TemplateId = Template.Id
                });
            }


            return Template;
        }

        private static string GetUsageId(string usageTerm)
        {
            return usageTerm switch
            {
                "Req" => ("CL-Compliance-T-1"),
                "Perm" => ("CL-Compliance-T-3"),
                "Exp" => ("CL-Compliance-T-2"),
                _ => "",
            };
        }

        private static string GetRoleId(string usageTerm)
        {
            return usageTerm switch
            {
                "Identifier" => ("CL-Role-T-1"),
                "Topic" => ("CL-Role-T-2"),
                "Record Qualifier" => ("CL-Role-T-3"),
                "Synonym Qualifier" => ("CL-Role-T-4"),
                "Variable Qualifier" => ("CL-Role-T-5"),
                "Timing" => ("CL-Role-T-6"),
                "Grouping Qualifier" => ("CL-Role-T-7"),
                "Result Qualifier" => ("CL-Role-T-8"),
                "Rule" => ("CL-Role-T-9"),
                _ => "",
            };
        }
    }
}

