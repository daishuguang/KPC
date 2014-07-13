using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	public class RouteSearchRequest
	{
        #region "Properties"
        [DataMember]
        public UserParticalModel User { get; set; }

        [DataMember]
        public RouteParticalModel Route { get; set; }

        [DataMember]
        public UserRole UserRole { get; set; }
        #endregion
	}
}