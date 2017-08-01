using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.SDTMHelper
{
    public class DataHandler
    {
        //----------------------------------------------------------EXTRACTING DATA

        /// <summary>
        /// Basic structure for a time-value profile
        /// the profileKeys and profileData variables will each
        /// only have one element in the list of values
        /// </summary>
        public struct profile
        {
            public List<keyElement> profileKeys;
            public List<valuePair> profileData;
        }
        /// <summary>
        /// Basic data structure of an <ID, value> pair
        /// used as key in data sets
        /// ID is a string identifier, essentially the Key
        /// </summary>
        public struct keyElement
        {
            public string ID;
            public List<object> Values;
        }
        /// <summary>
        /// Basic data structure of an <ID, value> pair
        /// used for values in a profile
        /// </summary>
        public struct valuePair
        {
            public object ID;
            public List<object> Values;
        }




        /// <summary>
        /// structure for a group of profiles
        /// the groupKeys variables will only have one element in the list of values
        /// </summary>
        public struct group
        {
            public List<keyElement> groupKeys;
            public List<keyElement> groupProfileKeys;
            public List<valuePair> groupProfileData;
        }


    }
}
