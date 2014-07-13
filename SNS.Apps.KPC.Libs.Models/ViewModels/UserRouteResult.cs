using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
    [DataContract]
    public class UserRouteResult
    {
        #region "Constructs"
		public UserRouteResult() { }

        public UserRouteResult(UserRoute entity)
        {
			if (entity != null)
			{
				this.User = entity.User;
				this.Route = entity.Route;
				this.UserRole = entity.UserRole.HasValue ? entity.UserRole.Value : Models.UserRole.Passenger;
			}
        }

		public UserRouteResult(DataStores.sp_Load_UserRoute_Newest_Result entity)
		{
			if (entity != null)
			{
				this.User = new User(entity);
				this.Route = new Route(entity);
				this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
			}
		}

		public UserRouteResult(DataStores.sp_Load_UserRoute_Newest_Disp_Result entity)
		{
			if (entity != null)
			{
				this.User = new User(entity);
				this.Route = new Route(entity);
				this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
			}
		}
        #endregion

        #region "Properties"
        [DataMember]
        public User User { get; set; }

        [DataMember]
        public Route Route { get; set; }

        [DataMember]
        public UserRole UserRole { get; set; }
        #endregion
    }
}
