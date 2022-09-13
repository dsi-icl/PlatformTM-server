using PlatformTM.Core.Domain.Model.Users.Queries;

namespace PlatformTM.Models.DTOs.Export
{
    public class DatasetField
    {
        public Query QueryObject { get; set; }
        public string ColumnHeader { get; set; }
        public bool ColumnHeaderIsEditable { get; set; } = true;
        private string _name;
        public string FieldName
        {
            get
            {
                if (QueryObject != null)
                {
                    if (QueryObject is ObservationQuery)
                    {
                        return ((ObservationQuery)QueryObject).TermName + (((ObservationQuery)QueryObject).PropertyLabel != null ? "" + ((ObservationQuery)QueryObject).PropertyLabel + "" : "");

                    }
                    return QueryObject.QueryFor + "[" + QueryObject.QuerySelectProperty + "]";
                }
                return _name;
            }
            set { _name = value; }
        }
        public string QueryObjectType { get; set; }

        public DatasetField()
        {
            ColumnHeader = FieldName;
        }
    }
}
