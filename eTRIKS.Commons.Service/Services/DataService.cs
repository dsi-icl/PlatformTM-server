using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services
{
    public class DataService
    {
        public DataService(IServiceUoW uoW)
        {
            //_activityServiceUnit = uoW;
            //_activityRepository = uoW.GetRepository<Activity, int>();
            //_dataSetRepository = uoW.GetRepository<Dataset, int>();

        }

        public List<Hashtable> getObservationsData(String studyId, List<string> observations)
        {
            //For each observation in observations 
            // query its results column from NoSQL collection
            // IF observations was an array like this one:
            //['BMI','Height','Weight']
            ///return an object that looks like that
            /*[
               { subjId: “xxx”,  BMI: 22.5, Height: 1.8, Weight: 80 },
             * { subjId: “xxx”,  BMI: 24.2, Height: 1.71, Weight: 70 },
             * { subjId: “xxx”,  BMI: 20.5, Height: 1.85, Weight: 83 },
             * { subjId: “xxx”,  BMI: 23.2, Height: 1.7, Weight: 80 },
             * { subjId: “xxx”,  BMI: 26, Height: 1.62, Weight: 75 },
             * { subjId: “xxx”,  BMI: 21, Height: 1.73, Weight: 80 },
            ]*/

            double[] heights = {1.8,1.71,1.85,1.7,1.62,1.73};
            int[] weights = { 80, 70, 83, 87, 65, 91 };
            double[] bmis = { 22.5, 24.2, 20.5, 23.2, 26, 21 };

            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;
            for (int i = 0; i < 6; i++)
            {
                obs = new Hashtable();
                obs.Add("subjId", "subj_"+i);
                obs.Add("BMI", bmis[i]);
                obs.Add("Height", heights[i]);
                obs.Add("Weight", weights[i]);
                observation_list.Add(obs);
            }

            return observation_list;

        }
    }
}
