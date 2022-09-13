using System.Collections;
using System.Collections.Generic;

namespace PlatformTM.Models.DTOs.Explorer
{
    public class SubjectExplorerDTO
    {
        public List<ObservationRequestDTO> scs { get; set; }
        public List<ObservationRequestDTO> tps { get; set; }
        public List<ObservationRequestDTO> des { get; set; }
        public Hashtable Xfdata { get; set; }

        public SubjectExplorerDTO()
        {
            scs = new List<ObservationRequestDTO>();
            tps = new List<ObservationRequestDTO>();
            des = new List<ObservationRequestDTO>();
            Xfdata = new Hashtable(){
                {"data", new List<Hashtable>()},
                {"keys", new HashSet<string>{"subjectId"}}
            };
        }

        public void AddKey(string key){
            ((HashSet<string>)Xfdata["keys"]).Add(key);
        }

        public void AddDataItem(Hashtable entry){
            ((List<Hashtable>)Xfdata["data"]).Add(entry);
        }
    }
}
