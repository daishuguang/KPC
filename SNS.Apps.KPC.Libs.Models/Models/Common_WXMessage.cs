using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models
{
	[DataContract]
	public class WXMessage
	{
		#region "Constructs"
		public WXMessage() { }

		public WXMessage(DataStores_Log.tbl_WXMessage entity)
		{
			if (entity != null)
			{
				Libs.Utils.CommonUtility.CopyTo(entity, this);

				this.MsgType = (entity.MsgType.HasValue) ? ((WXMessageType)Enum.Parse(typeof(WXMessageType), entity.MsgType.ToString())) : (WXMessageType.Text);
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public long ID { get; set; }

		[DataMember]
		public string From_OpenID { get; set; }

		[DataMember]
		public Nullable<long> From_UserID { get; set; }

		[DataMember]
		public string To_OpenID { get; set; }

		[DataMember]
		public Nullable<long> To_UserID { get; set; }

		[DataMember]
		public Nullable<long> MsgID { get; set; }

		[DataMember]
		public WXMessageType MsgType { get; set; }

		[DataMember]
		public string MsgContent { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format("From: {0}, To: {1}, MsgID: {2}, MsgType: {3}",
				((this.From_UserID != null && this.From_UserID.HasValue && this.From_UserID.Value != 0) ? this.From_UserID.Value.ToString() : this.From_OpenID),
				((this.To_UserID != null && this.To_UserID.HasValue && this.To_UserID.Value != 0) ? this.To_UserID.Value.ToString() : this.To_OpenID),
				(this.MsgID != null && this.MsgID.HasValue) ? this.MsgID.Value.ToString() : string.Empty,
				this.MsgType
			);
		}
		#endregion
	}

	[DataContract]
	public abstract class WXMessageBase
	{
		#region "Properties"
		[DataMember]
		public string OpenID { get; set; }

		[DataMember]
		public WXMessageType MsgType { get; set; }
		#endregion
	}

	[DataContract]
	public class WXMessageText : WXMessageBase
	{
		#region "Constructs"
		public WXMessageText() { this.MsgType = WXMessageType.Text; }
		#endregion

		#region "Properties"
		[DataMember]
		public string Content { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format(@"{{ ""touser"": ""{0}"", ""msgtype"": ""{1}"", ""text"": {{ ""content"": ""{2}"" }} }}", this.OpenID, this.MsgType.ToString().ToLower(), this.Content);
		}
		#endregion
	}

	[DataContract]
	public class WXMessageImage : WXMessageBase
	{
		#region "Constructs"
		public WXMessageImage() { this.MsgType = WXMessageType.Image; }
		#endregion

		#region "Properties"
		[DataMember]
		public string MediaID { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format(@"{{ ""touser"": ""{0}"", ""msgtype"": ""{1}"", ""image"": {{ ""media_id"": ""{2}"" }} }}", this.OpenID, this.MsgType.ToString().ToLower(), this.MediaID);
		}
		#endregion
	}

	[DataContract]
	public class WXMessageVoice : WXMessageBase
	{
		#region "Constructs"
		public WXMessageVoice() { this.MsgType = WXMessageType.Video; }
		#endregion

		#region "Properties"
		[DataMember]
		public string MediaID { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format(@"{{ ""touser"": ""{0}"", ""msgtype"": ""{1}"", ""voice"": {{ ""media_id"": ""{2}"" }} }}", this.OpenID, this.MsgType.ToString().ToLower(), this.MediaID);
		}
		#endregion
	}

	[DataContract]
	public class WXMessageVideo : WXMessageBase
	{
		#region "Constructs"
		public WXMessageVideo() { this.MsgType = WXMessageType.Video; }
		#endregion

		#region "Properties"
		[DataMember]
		public string MediaID { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Description { get; set; }
		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format(@"{{ ""touser"": ""{0}"", ""msgtype"": ""{1}"", ""video"": {{ ""media_id"": ""{2}"", ""title"": ""{3}"", ""description"": ""{4}"" }} }}", this.OpenID, this.MsgType.ToString().ToLower(), this.MediaID, this.Title, this.Description);
		}
		#endregion
	}

	[DataContract]
	public class WXMessageMusic : WXMessageBase
	{
		#region "Constructs"
		public WXMessageMusic() { this.MsgType = WXMessageType.Music; }
		#endregion

		#region "Properties"
		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string MusiCurl { get; set; }

		[DataMember]
		public string HQMusiCurl { get; set; }

		[DataMember]
		public string ThumbMediaID { get; set; }

		#endregion

		#region "Methods"
		public override string ToString()
		{
			return string.Format(@"{{ ""touser"": ""{0}"", ""msgtype"": ""{1}"", ""music"": {{ ""title"": ""{2}"", ""description"": ""{3}"", ""musicurl"": ""{4}"", ""hqmusicurl"": ""{5}"", ""thumb_media_id"": ""{6}"" }} }}", this.OpenID, this.MsgType.ToString().ToLower(), this.Title, this.Description, this.MusiCurl, this.HQMusiCurl, this.ThumbMediaID);
		}
		#endregion
	}

	public class WXMessageNews : WXMessageBase
	{

	}
}
