using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models
{
    public class SearchResultView
    {
        [DataMember]
        public UserParticalModel User { get; set; }

        [DataMember]
        public RouteParticalModel Route { get; set; }

        [DataMember]
        public UserRole UserRole { get; set; }
    }
}
