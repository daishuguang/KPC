using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MvcExtension;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;
using SNS.Apps.KPC.Libs.WeChatResponsor;

namespace SNS.Apps.KPC.WeChat.Controllers
{
	public class WeChatController : Base.BaseController
	{
		[HttpGet]
		[ActionName("Index")]
		public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
		{
			if (CheckSignature.Check(signature, timestamp, nonce, ConfigStore.WeChatAPISettings.WeChat_MP_Token))
			{
				return Content(echostr);
			}
			else
			{
				return Content("Invalid Signature!");
			}
		}

		[HttpPost]
		[ActionName("Index")]
		public ActionResult Post(string signature, string timestamp, string nonce, string echostr)
		{
			if (!CheckSignature.Check(signature, timestamp, nonce, ConfigStore.WeChatAPISettings.WeChat_MP_Token))
			{
				return Content("Invalid Signature!");
			}

			var client = CreateServiceClient<IRepositoryService>();
			var msgHandler = new WeChatHttpResponsor(Request.InputStream);

			try
			{
				//测试时可开启此记录，帮助跟踪数据
				//msgHandler.RequestDocument.Save(Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Request_" + messageHandler.RequestMessage.FromUserName + ".txt"));

				//执行微信处理过程
				msgHandler.Execute();

				//测试时可开启，帮助跟踪数据
				//msgHandler.ResponseDocument.Save(Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Response_" + messageHandler.ResponseMessage.ToUserName + ".txt"));

				return new WeixinResult(msgHandler);
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(string.Format(
					"Controller: {0}\r\nAction: {1}\r\nParameters: [{2}]\r\nException: {3}",
					 "WeChatResponsor",
					 "ActionResult Post(string signature, string timestamp, string nonce, string echostr)",
					 string.Format("signature: {0}, timestamp: {1}, nonce: {2}, echostr: {3}", signature, timestamp, nonce, echostr),
					 ex.ToString()
				));

				return null;
			}
			finally
			{
				CloseServiceClient(client);
			}
		}
	}
}