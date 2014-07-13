using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class User
	{
		#region "Constructs"
		public User() { }

		public User(DataStores.tbl_User entity)
		{
			if (entity != null)
			{
				Libs.Utils.CommonUtility.CopyTo(entity, this);

				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.UserRole = (entity.UserRole.HasValue) ? ((Models.UserRole)Enum.Parse(typeof(Models.UserRole), entity.UserRole.Value.ToString())) : (Models.UserRole.Passenger);
				this.Status = (entity.Status.HasValue) ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.Status.Value.ToString())) : (UserStatus.UnSubscribed);
				this.IsRegisterted = (this.Status == UserStatus.Registered);
			}
		}

		public User(DataStores.tbl_User_Import entity)
		{
			if (entity != null)
			{
				Libs.Utils.CommonUtility.CopyTo(entity, this);

				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.UserRole = (entity.UserRole.HasValue) ? ((Models.UserRole)Enum.Parse(typeof(Models.UserRole), entity.UserRole.Value.ToString())) : (Models.UserRole.Passenger);
				this.Status = UserStatus.UnSubscribed;
				this.IsRegisterted = (this.Status == UserStatus.Registered);
			}
		}

		public User(DataStores.tbl_User_External entity)
		{
			if (entity != null)
			{
				Libs.Utils.CommonUtility.CopyTo(entity, this);

				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.UserRole = (entity.UserRole.HasValue) ? ((Models.UserRole)Enum.Parse(typeof(Models.UserRole), entity.UserRole.Value.ToString())) : (Models.UserRole.Passenger);
				this.Status = (entity.Status.HasValue) ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.Status.Value.ToString())) : (UserStatus.UnSubscribed);
				this.IsRegisterted = (this.Status == UserStatus.Registered);
			}
		}

		public User(DataStores.sp_Load_UserRoute_By_RouteID_Result entity)
		{
			if (entity != null)
			{
				this.ID = entity.UserID.Value;
				this.OpenID = entity.UserOpenID;
				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.NickName = entity.User_NickName;
				this.WeChatID = entity.User_WeChatID;
				this.Mobile = entity.User_Mobile;
				this.LicencePlateNumber = entity.User_LicencePlateNumber;
				this.Status = (entity.User_Status.HasValue) ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.User_Status.Value.ToString())) : (UserStatus.UnSubscribed);
				this.IsRegisterted = this.Status == UserStatus.Registered;
				this.PortraitsUrl = entity.User_PortraitsUrl;
				this.PortraitsThumbUrl = entity.User_PortraitsThumbUrl;
			}
		}

		public User(DataStores.sp_Load_UserRoute_By_RouteIDs_Result entity)
		{
			if (entity != null)
			{
				this.ID = entity.UserID.Value;
				this.OpenID = entity.UserOpenID;
				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.NickName = entity.User_NickName;
				this.WeChatID = entity.User_WeChatID;
				this.Mobile = entity.User_Mobile;
				this.LicencePlateNumber = entity.User_LicencePlateNumber;
				this.Status = (entity.User_Status.HasValue) ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.User_Status.Value.ToString())) : (UserStatus.UnSubscribed);
				this.IsRegisterted = this.Status == UserStatus.Registered;
				this.PortraitsUrl = entity.User_PortraitsUrl;
				this.PortraitsThumbUrl = entity.User_PortraitsThumbUrl;
			}
		}

		public User(DataStores.sp_Load_UserRoute_By_UserID_Result entity)
		{
			if (entity != null)
			{
				this.ID = entity.UserID;
				this.OpenID = entity.UserOpenID;
				this.UserGUID = Guid.Parse(entity.UserGUID);
				this.NickName = entity.User_NickName;
				this.WeChatID = entity.User_WeChatID;
				this.Mobile = entity.User_Mobile;
				this.LicencePlateNumber = entity.User_LicencePlateNumber;
				this.Status = (entity.User_Status.HasValue) ? ((UserStatus)Enum.Parse(typeof(UserStatus), entity.User_Status.Value.ToString())) : (UserStatus.UnSubscribed);
				this.IsRegisterted = this.Status == UserStatus.Registered;
				this.PortraitsUrl = entity.User_PortraitsUrl;
				this.PortraitsThumbUrl = entity.User_PortraitsThumbUrl;
			}
		}

		public User(DataStores.sp_Load_UserRoute_Newest_Result entity)
		{
			if (entity != null)
			{
				this.ID = entity.UserID;
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

		public User(DataStores.sp_Load_UserRoute_Newest_Disp_Result entity)
		{
			if (entity != null)
			{
				this.ID = entity.UserID;
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
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public string FakeID { get; set; }

		[DataMember]
		public Guid UserGUID { get; set; }

		[DataMember]
		public string NickName { get; set; }

		[DataMember]
		public Nullable<bool> Gender { get; set; }

		[IgnoreDataMember]
		private string Name { get; set; }

		[IgnoreDataMember]
		private string IdentityNo { get; set; }

		[IgnoreDataMember]
		private Nullable<DateTime> Birthday { get; set; }

		[DataMember]
		public string Country { get; set; }

		[DataMember]
		public string Province { get; set; }

		[DataMember]
		public string City { get; set; }

		[IgnoreDataMember]
		public string District { get; set; }

		[IgnoreDataMember]
		public string Location { get; set; }

		[DataMember]
		public string Mobile { get; set; }

		[IgnoreDataMember]
		private string Password { get; set; }

		[DataMember]
		public string WeChatID { get; set; }

		[DataMember]
		public string QQ { get; set; }

		[DataMember]
		public string LicencePlateNumber { get; set; }

		[DataMember]
		public string PortraitsUrl { get; set; }

		[DataMember]
		public string PortraitsThumbUrl { get; set; }

		//[DataMember]
		//public Nullable<bool> IsSynced { get; set; }

		//[DataMember]
		//public Nullable<bool> EnableNotify { get; set; }

		[DataMember]
		public Nullable<bool> IsExtended { get; set; }

		[DataMember]
		public Nullable<long> ExtendChannel { get; set; }

		[DataMember]
		public Nullable<long> RefID { get; set; }
		#endregion

		#region "Properties"
		[DataMember]
		public UserRole UserRole { get; set; }

		[DataMember]
		public UserStatus Status { get; set; }
		#endregion

		#region "Properties"
		[DataMember]
		public bool IsRegisterted { get; set; } 
		#endregion
	}

	[DataContract]
	public sealed class UserPartical : User
	{
		#region "Properties"
		[IgnoreDataMember]
		private new long ID { get; set; }  
		#endregion
	}

	[DataContract]
	public sealed class UserSecurity : User
	{
		#region "Constructs"
		public UserSecurity(User instance) 
		{
 			if (instance != null)
			{
				Utils.CommonUtility.CopyTo(instance, this);
			}
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public string Password { get; set; }
		#endregion
	}

	[DataContract]
	public sealed class UserPrivy : User
	{
		#region "Constructs"
		public UserPrivy(User instance) 
		{
 			if (instance != null)
			{
				Utils.CommonUtility.CopyTo(instance, this);
			}
		} 
		#endregion

		#region "Properties"
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string IdentityNo { get; set; }

		[DataMember]
		public Nullable<DateTime> Birthday { get; set; }

		[DataMember]
		public string District { get; set; }

		[DataMember]
		public string Location { get; set; }
		#endregion
	}
}
