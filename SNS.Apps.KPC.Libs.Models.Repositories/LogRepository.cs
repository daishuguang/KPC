using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public sealed class LogRepository : Base.RepositoryBase<LogRepository>
	{
		#region "Constructs"
		private LogRepository() { } 
		#endregion

		#region "Methods: GetLatestWXMsg"
		public WXMessage GetLatestWXMsg(long userID, bool inForce = false)
		{
			var instance = default(WXMessage);

			if (inForce)
			{
				instance = GetLatestWXMsg_FromDB(userID);

				SetMem(instance, CNSTR_MEMCACHEKEY_WXMESSAGE_USERID, userID);
			}
			else
			{
				instance = GetMem(() => { return GetLatestWXMsg_FromDB(userID); }, CNSTR_MEMCACHEKEY_WXMESSAGE_USERID, userID);
			}

			return instance;
		}

		public WXMessage GetLatestWXMsg(string openID, bool inForce = false)
		{
			var instance = default(WXMessage);

			if (inForce)
			{
				instance = GetLatestWXMsg_FromDB(openID);

				SetMem(instance, CNSTR_MEMCACHEKEY_WXMESSAGE_OPENID, openID);
			}
			else
			{
				instance = GetMem(() => { return GetLatestWXMsg_FromDB(openID); }, CNSTR_MEMCACHEKEY_WXMESSAGE_OPENID, openID);
			}

			return instance;
		}

		WXMessage GetLatestWXMsg_FromDB(long userID)
		{
			using (var ctx = new DataStores_Log.KPCLogModels())
			{
				var result = ctx.tbl_WXMessage.Where(p => p.From_UserID == userID).OrderByDescending(p => p.ID).FirstOrDefault();

				return (result != null) ? (new WXMessage(result)) : (null);
			}
		}

		WXMessage GetLatestWXMsg_FromDB(string openID)
		{
			using (var ctx = new DataStores_Log.KPCLogModels())
			{
				var result = ctx.tbl_WXMessage.Where(p => string.Compare(p.From_OpenID, openID, StringComparison.InvariantCultureIgnoreCase) == 0).OrderByDescending(p => p.ID).FirstOrDefault();

				return (result != null) ? (new WXMessage(result)) : (null);
			}
		}
		#endregion

		#region "Methods: ArchiveWXMsg"
		public void ArchiveWXMsg(WXMessage msg)
		{
			using (var ctx = new DataStores_Log.KPCLogModels())
			{
				var entity = ctx.tbl_WXMessage.Create();

				#region "Set Fields"
				entity.From_UserID = msg.From_UserID;
				entity.From_OpenID = msg.From_OpenID;
				entity.To_UserID = msg.To_UserID;
				entity.To_OpenID = msg.To_OpenID;
				entity.MsgID = msg.MsgID;
				entity.MsgType = (int)msg.MsgType;
				entity.MsgContent = msg.MsgContent;
				entity.CreateDate = DateTime.Now; 
				#endregion

				ctx.tbl_WXMessage.Add(entity);
				ctx.SaveChanges();
			}
		} 
		#endregion
	}
}
