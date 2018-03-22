using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;

namespace eTRIKS.Commons.Service.DTOs
{
    public class DataTemplateMap
    {
        public string Domain { get; set; }
        public string ObservationName { get; set; }
        //public List<Dictionary<string,List<VariableMap>>> VarTypes{ get; set; }
        public List<VariableType> VarTypes { get; set; } 
        public List<string> TopicColumns { get; set; }

        public DataTemplateMap()
        {
            TopicColumns = new List<string>(){"1"};
        }

        public class VariableType
        {
            public string name { get; set; }
            public List<VariableMap> vars { get; set; }
        }

        public class VariableMap
        {
            public string Label { get; set; }
            public string Description { get; set; }
            public string ShortName { get; set; }
            public List<ColHeaderDTO> MapToColList { get; set; }
            public string DataType { get; set; }
            public List<string> MapToStringValueList { get; set; }
 
        }
    }


    public class AnnotatedFile{

        public AnnotatedColumn SubjectIdColumn { get; set; }
        public AnnotatedColumn StudyIdColumn { get; set; }
        public DataFile SourceFile { get; set; }
        public Dataset TargetDataset { get; set; }
        //public List<AnnotatedColumn> TopicColumns { get; set; }
        public AnnotatedColumn TopicColumn { get; set; }
        public bool HasTimeSeriesColumns { get; set; } //we need to identify if there are more than column with qualifier data per row
        public Dictionary<string,AnnotatedColumn> TimeSeriesColumns { get; set; } //this will have columns that mapped to one variable but each has a different timepoint value for it
        // the string represents the name of the variable that these columsna are mapped to



        /**
         * This will be prepopulated with the variable names expected from the template
         */
        //public List<AnnotatedColumn> TemporalAnnotations { get; set; }

        
    }

    public class AnnotatedColumn{

        DataColumn SourceColumn { get; set; }
        string MappedVariable { get; set; }
        string DataType { get; set; }
        int Position { get; set; }
		//in order to know which property of the sdtm row to give this value to, we will have to have a method in the 
		//sdtm descriptor to take a variable name and return a property of the class OTHERWISE we use a temp structure for the SDTMrow 
		// a simple dictionary with  varaibel names and values and then send it to the descriptor to interpret it. 

		//... that is for infered information that would come from the columns header 
		Dictionary<string,string> LinkedValues { get; set; } //VSDY,1 // VSTPT,12) 

	}

    public class TopicAnnotation{
        List<AnnotatedColumn> Qualifiers { get; set; }
    }

    public class TopicColumn { 
        
    }

    public class TopicValueAnnotation{
        string TopicValue { get; set; }
        string MappedVariable { get; set; } //VSTESTCD
    }

    public class QualifierColumn{
        
    }


}
