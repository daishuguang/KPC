using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

using Senparc.Weixin.MP.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;
using SNS.Apps.KPC.Libs.IServices;

namespace SNS.Apps.KPC.Libs.WeChatResponsor
{
	public partial class WeChatHttpResponsor : MessageHandler<MessageContext>
	{
		#region "Constructs"
		public WeChatHttpResponsor(Stream inputStream)
			: base(inputStream)
		{
			//这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
			//比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
			WeixinContext.ExpireMinutes = 3;
			WeixinContext.MaxRecordCount = 3;
		}
		#endregion

		#region "Events"
		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestMessage"></param>
		/// <returns></returns>
		public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				switch (requestMessage.EventKey)
				{
					#region "菜单：我要拼车
					case "Event_Key_Plaza":
						{
							return SendMsg_Menu_Plaza(requestMessage, client);
						}
					case "Event_Key_Around":
						{
							return SendMsg_Menu_Around(requestMessage, client);
						}
					#endregion

					#region "菜单：个人中心"
					case "Event_Key_MyOrders":
						{
							return SendMsg_Menu_MyOrder(requestMessage, client);
						}
					case "Event_Key_MyRoutes":
						{
							return SendMsg_Menu_MyRoute(requestMessage, client);
						}
					#endregion

					#region "菜单：更多服务"
					case "Event_Key_Agent":
						{
							return SendMsg_Menu_Agent(requestMessage, client);
						}
					case "Event_Key_Feedback":
						{
							return SendMsg_Menu_Feedback(requestMessage, client);
						}
					#endregion
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("Method: {0}\r\nFromUser: {1}, ToUser: {2}, EventKey: {3}, MsgID: {4}, MsgType: {5}\r\nException: {6}",
												"IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)",
												requestMessage.FromUserName,
												requestMessage.ToUserName,
												requestMessage.EventKey,
												requestMessage.MsgId,
												requestMessage.MsgType,
												ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(client);
			}

			return null;
		}

		public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
		{
			return null;
		}

		public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				var accessor = client.GetUser(requestMessage.FromUserName);

				if (accessor != null)
				{
					client.SetUserTrack(accessor.UserGUID, Point.Convert(new Point { Longitude = requestMessage.Longitude, Latitude = requestMessage.Latitude }, PointCoordType.GoogleMap_gcj0211, PointCoordType.BaiduMap_bd0911));
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("Method: {0}\r\nFromUser: {1}, ToUser: {2}, EventKey: {3}, MsgID: {4}, MsgType: {5}\r\nException: {6}",
												"IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)",
												requestMessage.FromUserName,
												requestMessage.ToUserName,
												requestMessage.EventKey,
												requestMessage.MsgId,
												requestMessage.MsgType,
												ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(client);
			}

			return null;
		}

		/// <summary>
		/// 订阅（关注）事件
		/// </summary>
		/// <returns></returns>
		public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
		{
			var clientR = CreateServiceClient<IRepositoryService>();
			var clientW = CreateServiceClient<IWeChatService>();

			try
			{
				var refID = ParseQRSceneID(requestMessage);
				var accessor = clientR.Subscribe(requestMessage.FromUserName, null, refID);
				
				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					if (refID.HasValue)
					{
						if (accessor.RefID.HasValue && accessor.RefID.Value == refID.Value && clientR.CheckIsSubscribeFirst(accessor.ID))
						{
							// 首次加关注，向代理商发送微信通知
							var agent = clientR.GetPromoter(refID);

							if (agent != null)
							{
								clientW.SendMessage(agent.Promoter.OpenID, string.Format("{0} 您好，您推荐的用户 {1} 已成功首次加关注快拼车公众账号！", agent.Promoter.NickName, accessor.NickName), false);
							}
						}

						DBLogger.Instance.Info(new Utils.Logger.Base.WXLogMessage { UserID = requestMessage.FromUserName, Message = string.Format("User '{0}' subscribe, from Promotion ID: {1}", requestMessage.FromUserName, refID.Value) });
					}
					else
					{
						DBLogger.Instance.Info(new Utils.Logger.Base.WXLogMessage { UserID = requestMessage.FromUserName, Message = string.Format("User '{0}' subscribe", requestMessage.FromUserName) });
					}
				}

				return SendMsg_Menu_Subscribe(requestMessage);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("Method: {0}\r\nFromUser: {1}, ToUser: {2}, EventKey: {3}, MsgID: {4}, MsgType: {5}\r\nException: {6}",
												"IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)",
												requestMessage.FromUserName,
												requestMessage.ToUserName,
												requestMessage.EventKey,
												requestMessage.MsgId,
												requestMessage.MsgType,
												ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(clientR);
				CloseServiceClient(clientW);
			}

			return null;
		}

		/// <summary>
		/// 退订
		/// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
		/// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
		/// </summary>
		/// <returns></returns>
		public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				// Unsubscribe User from DB
				client.Unsubscribe(requestMessage.FromUserName);

				if (ConfigStore.CommonSettings.Trace_Mode)
				{
					DBLogger.Instance.Info(new Utils.Logger.Base.WXLogMessage { UserID = requestMessage.FromUserName, Message = string.Format("User '{0}' unsubscribe", requestMessage.FromUserName) });
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("Method: {0}\r\nFromUser: {1}, ToUser: {2}, EventKey: {3}, MsgID: {4}, MsgType: {5}\r\nException: {6}",
												"IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)",
												requestMessage.FromUserName,
												requestMessage.ToUserName,
												requestMessage.EventKey,
												requestMessage.MsgId,
												requestMessage.MsgType,
												ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(client);
			}

			return null;
		}
		#endregion

		#region "Private Methods"
		void Archive_WeChatMessage(object state)
		{
			var clientL = CreateServiceClient<ILogService>();

			try
			{
				var requestMsg = Libs.Utils.CommonUtility.GetPropertyValue<object, RequestMessageBase>(state, "RequestMsg");
				var msgType = Libs.Utils.CommonUtility.GetPropertyValue<object, WXMessageType>(state, "MsgType");

				clientL.ArchiveWXMsg(new WXMessage 
				{
 					From_OpenID = requestMsg.FromUserName,
					To_OpenID = requestMsg.ToUserName,
					MsgID = requestMsg.MsgId,
					MsgType = msgType,
					MsgContent = requestMsg.ConvertEntityToXmlString()
				});
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Model: [{0}], Method: [{1}]\r\nException: {2}",
					"WeChatResponsor",
					"void Archive_WeChatMessage(object state)",
					ex.ToString()
				);
			}
			finally
			{
				CloseServiceClient(clientL);
			}
		}

		int? ParseQRSceneID(RequestMessageEvent_Subscribe requestMessage)
		{
			var xmlDoc = new System.Xml.XmlDocument();

			xmlDoc.LoadXml(requestMessage.ConvertEntityToXmlString());

			var xmlNode = xmlDoc.SelectSingleNode("//EventKey");

			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				var xmlComment = (XmlCDataSection)xmlNode.ChildNodes[0];

				if (xmlComment != null)
				{
					if (!string.IsNullOrEmpty(xmlComment.Data))
					{
						var sceneID = 0;

						if (int.TryParse(xmlComment.Data.Substring(xmlComment.Data.IndexOf("_") + 1), out sceneID))
						{
							return sceneID;
						}
					}
				}
			}

			return (int?)null;
		}
		#endregion
	}
}
