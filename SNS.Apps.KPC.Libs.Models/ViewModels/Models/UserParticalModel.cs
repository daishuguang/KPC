using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Runtime.Serialization;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class UserParticalModel
	{
		#region "Fields"
		const string CNSTR_PORTRAITS_DEFAULT = "/Content/portraits/default.gif";
		#endregion

		#region "Constructs"
		public UserParticalModel() { }

		public UserParticalModel(User entity)
		{
			if (entity != null)
			{
				this.OpenID = entity.OpenID;
				this.UserGUID = entity.UserGUID;
				this.NickName = entity.NickName;
				this.Gender = entity.Gender;
				this.Mobile = entity.Mobile;
				this.WeChatID = entity.WeChatID;
				this.UserRole = entity.UserRole;
				this.LicencePlateNumber = entity.LicencePlateNumber;
				this.PortraitsUrl = entity.PortraitsUrl;
				this.PortraitsThumbUrl = entity.PortraitsThumbUrl;
				this.Status = entity.Status;
				this.IsRegisterted = entity.IsRegisterted;
			}
		}

		public UserParticalModel(DataStores.sp_Load_UserRoute_Newest_Result entity)
		{
			if (entity != null)
			{
				this.OpenID = entity.UserOpenID;
				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.NickName = entity.User_NickName;
				this.Gender = entity.User_Gender ?? true;
				this.Mobile = entity.User_Mobile;
				this.WeChatID = entity.User_WeChatID;
				this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
				this.LicencePlateNumber = entity.User_LicencePlateNumber;
				this.PortraitsUrl = entity.User_PortraitsUrl;
				this.PortraitsThumbUrl = entity.User_PortraitsThumbUrl;
				this.Status = entity.User_Status.HasValue ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.User_Status.Value.ToString())) : UserStatus.Subscribed;
				this.IsRegisterted = this.Status == UserStatus.Registered;
			}
		}

		public UserParticalModel(DataStores.sp_Load_UserRoute_Newest_Disp_Result entity)
		{
			if (entity != null)
			{
				this.OpenID = entity.UserOpenID;
				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.NickName = entity.User_NickName;
				this.Gender = entity.User_Gender ?? true;
				this.Mobile = entity.User_Mobile;
				this.WeChatID = entity.User_WeChatID;
				this.UserRole = (UserRole)Enum.Parse(typeof(UserRole), entity.UserRole.ToString());
				this.LicencePlateNumber = entity.User_LicencePlateNumber;
				this.PortraitsUrl = entity.User_PortraitsUrl;
				this.PortraitsThumbUrl = entity.User_PortraitsThumbUrl;
				this.Status = entity.User_Status.HasValue ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.User_Status.Value.ToString())) : UserStatus.Subscribed;
				this.IsRegisterted = this.Status == UserStatus.Registered;
			}
		}
		#endregion

		#region "Properties"
		//[DataMember]
		//public long UserID { get; set; }                   

		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public Guid UserGUID { get; set; }

		[DataMember]
		public Nullable<bool> Gender { get; set; }

		[DataMember]
		public string NickName { get; set; }

		[DataMember]
		public string Mobile { get; set; }

		[DataMember]
		public string WeChatID { get; set; }

		[DataMember]
		public UserRole UserRole { get; set; }

		[DataMember]
		public string LicencePlateNumber { get; set; }

		[DataMember]
		public string PortraitsUrl { get; set; }

		[DataMember]
		public string PortraitsThumbUrl { get; set; }

		[DataMember]
		public UserStatus Status { get; set; }

		[DataMember]
		public bool IsRegisterted { get; set; }
		#endregion
	}
}
