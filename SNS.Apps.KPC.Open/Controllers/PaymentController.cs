using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Xml;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;
using Com.Alipay;

namespace SNS.Apps.KPC.Open.Controllers
{
	public class PaymentController : Base.BaseController
    {
		#region "Action: Pay"
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult Pay()
		{
			var folio = Request.Form["Folio"];
			var payMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), Request.Form["pay_ways"]);

			if (!string.IsNullOrEmpty(Request.Form["cash_ways"]))
			{
				var cashMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), Request.Form["cash_ways"]);

				return RedirectToAction("Pay", "Payment", new { folio = folio, payMethod = ((int)payMethod | (int)cashMethod) });
			}

			return RedirectToAction("Pay", "Payment", new { folio = folio, payMethod = (int)payMethod });
		}

		[Authorize]
		public ActionResult Pay(string folio, int payMethod)
		{
			var clientR = CreateServiceClient<IRepositoryService>();

			try
			{
				#region "验证参数"
				if (string.IsNullOrEmpty(folio))
				{
					throw new ArgumentNullException("未指定参数： Folio");
				}

				var uo = clientR.GetUserOrder(folio, true);

				if (uo == null)
				{
					throw new Exception(string.Format("未能找到 Folio 为 '{0}' 的订单！", folio));
				}

				if (!this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID) && !this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID))
				{
					throw new UnauthorizedAccessException("您未授权访问该拼单信息！");
				}
				#endregion

				#region "验证状态、角色"
				if (uo.Status != OrderStatus.PendingForPayment)
				{
					return RedirectToAction("Detail", new { folio = folio });
				}

				var isRequestor = this.CurrentUser.UserGUID.Equals(uo.Requestor.UserGUID);
				var isSupplier = this.CurrentUser.UserGUID.Equals(uo.Supplier.UserGUID);

				if ((isRequestor && uo.RequestorRole != UserRole.Passenger) ||
					(isSupplier && uo.SupplierRole != UserRole.Passenger))
				{
					return RedirectToAction("Detail", new { folio = folio });
				}
				#endregion

				if (payMethod == (int)PaymentMethod.DirectPay)
				{
					#region "线下支付"
					if (clientR.ExecutePayment(new PaymentSubmitRequest { ID = uo.ID, PayMethod = payMethod, PayAmount = uo.Charge, PayStatus = PaymentStatus.Pending }))
					{
						TempData["SuccessMsg"] = string.Format(@"您已选择 “线下支付”，请在拼车完成时向车主 “{0}” 支付{1}，谢谢！", (isRequestor) ? (uo.Supplier.NickName) : (uo.Requestor.NickName), (uo.Charge != null && uo.Charge.HasValue) ? (string.Format(" “{0}” 元拼车费用", uo.Charge.Value.ToString("N2"))) : ("所需拼车费用"));

						return RedirectToAction("Detail", "Order", new { folio = uo.Folio });
					}
					#endregion
				}
				else
				{
					#region "计算支付额度"
					var amount = 0.0M;

					if ((payMethod & (int)PaymentMethod.OnlinPay_Deposit) != 0)
					{
						if (!uo.Charge.HasValue || uo.Charge.Value == 0)
						{
							amount = 5.0M;
						}
						else
						{
							amount = Math.Ceiling(uo.Charge.Value * 0.1M);
						}
					}
					else if ((payMethod & (int)PaymentMethod.OnlinPay_FullCash) != 0)
					{
						if (!uo.Charge.HasValue || uo.Charge.Value == 0)
						{
							TempData["ErrorMsg"] = "很抱歉，“面议”选项不支持全额支付，请选择线下支付方式！";

							return RedirectToAction("Detail", "Order", new { folio = folio }); ;
						}
						else
						{
							amount = uo.Charge.Value;
						}
					} 
					#endregion

					switch ((PaymentMethod)Enum.Parse(typeof(PaymentMethod), ((int)payMethod & 0xF0).ToString()))
					{
						case PaymentMethod.Alipay:
							{
								#region "支付宝支付"
								if (clientR.ExecutePayment(new PaymentSubmitRequest { ID = uo.ID, PayMethod = payMethod, PayAmount = amount, PayStatus = PaymentStatus.Pending }))
								{
									if (ConfigStore.CommonSettings.Debug_Mode)
									{
										amount = 0.1M;
									}

									return RedirectToAction("Alipay", new { folio = folio, amount = amount, memo = string.Format("{0} {1} ~ {2}", uo.StartDate.Value.ToString("MM月dd日"), (!string.IsNullOrEmpty(uo.Route.From_Location) ? uo.Route.From_Location.Trim() : string.Format("{0}{1}", uo.Route.From_City, uo.Route.From_District)), (!string.IsNullOrEmpty(uo.Route.To_Location) ? uo.Route.To_Location.Trim() : string.Format("{0}{1}", uo.Route.To_City, uo.Route.To_District))) });
								}
								#endregion
							}
							break;
						case PaymentMethod.WXPay:
							{
								#region "微信支付"
								// 
								#endregion
							}
							break;
					}
				}

				TempData["ErrorMsg"] = @"很抱歉，支付遇到问题，未能成功完成您的请求，请稍后再试！";

				return RedirectToAction("Detail", "Order", new { folio = uo.Folio });
			}
			finally
			{
				CloseServiceClient(clientR);
			}
		}
		#endregion

		#region "Action: Alipay"
		[Authorize]
		public ActionResult Alipay(string folio, decimal amount, string memo)
		{
			//支付宝网关地址
			string GATEWAY_NEW = ConfigStore.PaymentSettings.AlipaySettings.Payment_Alipay_Gateway_URI;

			#region ////////////////////////////////////////////调用授权接口alipay.wap.trade.create.direct获取授权码token////////////////////////////////////////////

			//返回格式
			string format = "xml";
			//必填，不需要修改

			//返回格式
			string v = "2.0";
			//必填，不需要修改

			//请求号
			string req_id = string.Format("{0}-{1:D4}", DateTime.Now.ToString("yyyyMMddHHmmss"), DateTime.Now.Ticks);
			//必填，须保证每次请求都是唯一

			//req_data详细信息

			//服务器异步通知页面路径
			string notify_url = string.Format("{0}/payment/alipay_notify", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'));
			//需http://格式的完整路径，不允许加?id=123这类自定义参数

			//页面跳转同步通知页面路径
			string call_back_url = string.Format("{0}/payment/alipay_callback", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'));
			//需http://格式的完整路径，不允许加?id=123这类自定义参数

			//操作中断返回地址
			string merchant_url = string.Format("{0}/order/detail/{1}", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'), folio);
			//用户付款中途退出返回商户的地址。需http://格式的完整路径，不允许加?id=123这类自定义参数

			//卖家支付宝帐户
			string seller_email = ConfigStore.PaymentSettings.AlipaySettings.Payment_Alipay_Seller_Account;
			//必填

			//商户订单号
			string out_trade_no = folio;
			//商户网站订单系统中唯一订单号，必填

			//订单名称
			string subject = (!string.IsNullOrEmpty(memo)) ? (string.Format("快拼车-拼车费用（{0}）", memo)) : ("快拼车-拼车费用");
			//必填

			//付款金额
			string total_fee = amount.ToString();
			//必填

			//请求业务参数详细
			string req_dataToken = "<direct_trade_create_req><notify_url>" + notify_url + "</notify_url><call_back_url>" + call_back_url + "</call_back_url><seller_account_name>" + seller_email + "</seller_account_name><out_trade_no>" + out_trade_no + "</out_trade_no><subject>" + subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" + merchant_url + "</merchant_url></direct_trade_create_req>";
			//必填

			//把请求参数打包成数组
			Dictionary<string, string> sParaTempToken = new Dictionary<string, string>();
			sParaTempToken.Add("partner", Config.Partner);
			sParaTempToken.Add("_input_charset", Config.Input_charset.ToLower());
			sParaTempToken.Add("sec_id", Config.Sign_type.ToUpper());
			sParaTempToken.Add("service", "alipay.wap.trade.create.direct");
			sParaTempToken.Add("format", format);
			sParaTempToken.Add("v", v);
			sParaTempToken.Add("req_id", req_id);
			sParaTempToken.Add("req_data", req_dataToken);

			//建立请求
			string sHtmlTextToken = Submit.BuildRequest(GATEWAY_NEW, sParaTempToken);
			//URLDECODE返回的信息
			Encoding code = Encoding.GetEncoding(Config.Input_charset);
			sHtmlTextToken = HttpUtility.UrlDecode(sHtmlTextToken, code);

			//解析远程模拟提交后返回的信息
			Dictionary<string, string> dicHtmlTextToken = Submit.ParseResponse(sHtmlTextToken);

			//获取token
			string request_token = dicHtmlTextToken["request_token"];
			#endregion


			#region ////////////////////////////////////////////根据授权码token调用交易接口alipay.wap.auth.authAndExecute////////////////////////////////////////////

			//业务详细
			string req_data = "<auth_and_execute_req><request_token>" + request_token + "</request_token></auth_and_execute_req>";
			//必填

			//把请求参数打包成数组
			Dictionary<string, string> sParaTemp = new Dictionary<string, string>();
			sParaTemp.Add("partner", Config.Partner);
			sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
			sParaTemp.Add("sec_id", Config.Sign_type.ToUpper());
			sParaTemp.Add("service", "alipay.wap.auth.authAndExecute");
			sParaTemp.Add("format", format);
			sParaTemp.Add("v", v);
			sParaTemp.Add("req_data", req_data);

			//建立请求
			string sHtmlText = Submit.BuildRequest(GATEWAY_NEW, sParaTemp, "get", "确认");
			//Response.Write(sHtmlText);

			return Content(sHtmlText);
			#endregion
		}

		[HttpPost]
		[AcceptVerbs(HttpVerbs.Post)]
		[ValidateInput(false)]
		public ActionResult Alipay_Notify()
		{
			Dictionary<string, string> sPara = GetRequestPost();

			if (sPara.Count > 0)//判断是否有带返回参数
			{
				Notify aliNotify = new Notify();
				bool verifyResult = aliNotify.VerifyNotify(sPara, Request.Form["sign"]);

				if (verifyResult)//验证成功
				{
					/////////////////////////////////////////////////////////////////////////////////////////////////////////////
					//请在这里加上商户的业务逻辑程序代码


					//——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
					//获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

					//解密（如果是RSA签名需要解密，如果是MD5签名则下面一行清注释掉）
					//sPara = aliNotify.Decrypt(sPara);

					//XML解析notify_data数据

					var clientR = CreateServiceClient<IRepositoryService>();

					try
					{
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(sPara["notify_data"]);

						//商户订单号
						var orderFolio = xmlDoc.SelectSingleNode("/notify/out_trade_no").InnerText;

						//支付宝交易号
						var payFolio = xmlDoc.SelectSingleNode("/notify/trade_no").InnerText;

						//交易状态
						var payStatus = xmlDoc.SelectSingleNode("/notify/trade_status").InnerText;

						#region "验证参数，状态"
						if (string.IsNullOrEmpty(orderFolio))
						{
							DBLogger.Instance.Error("无效订单号：空的订单号");

							return Content("无效订单号：空的订单号");
						}

						var uo = clientR.GetUserOrder(orderFolio, true);

						if (uo == null)
						{
							DBLogger.Instance.ErrorFormat("无效订单号：未找到订单号为‘{0}’的订单", orderFolio);

							return Content(string.Format("无效订单号：未找到订单号为‘{0}’的订单", orderFolio));
						}

						if (uo.Status != OrderStatus.PendingForPayment)
						{
							DBLogger.Instance.ErrorFormat("订单已完成支付，无需重复支付: {0}", orderFolio);

							return Content("订单已完成支付，无需重复支付");
						}
						#endregion

						if (payStatus == "TRADE_FINISHED")
						{
							//判断该笔订单是否在商户网站中已经做过处理
							//如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
							//如果有做过处理，不执行商户的业务程序

							//注意：
							//该种交易状态只在两种情况下出现
							//1、开通了普通即时到账，买家付款成功后。
							//2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。

							if (clientR.FinishPayment(uo.ID, payFolio))
							{
								DBLogger.Instance.InfoFormat("订单号：{0}，交易号：{1}，交易状态：{2}，支付已成功，更新订单状态已成功！", orderFolio, payFolio, payStatus);

								return Content("success");
							}
							else
							{
								DBLogger.Instance.ErrorFormat("订单号：{0}，交易号：{1}，交易状态：{2}，支付已成功，更新订单状态失败！", orderFolio, payFolio, payStatus);
							}
						}
						else if (payStatus == "TRADE_SUCCESS")
						{
							//判断该笔订单是否在商户网站中已经做过处理
							//如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
							//如果有做过处理，不执行商户的业务程序

							//注意：
							//该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。

							if (clientR.FinishPayment(uo.ID, payFolio))
							{
								DBLogger.Instance.InfoFormat("订单号：{0}，交易号：{1}，交易状态：{2}，支付已成功，更新订单状态已成功！", orderFolio, payFolio, payStatus);

								return Content("success");
							}
							else
							{
								DBLogger.Instance.ErrorFormat("订单号：{0}，交易号：{1}，交易状态：{2}，支付已成功，更新订单状态失败！", orderFolio, payFolio, payStatus);
							}
						}
						else
						{
							DBLogger.Instance.ErrorFormat("订单号：{0}，交易号：{1}，交易状态：{2}，支付失败！", orderFolio, payFolio, payStatus);

							return Content(payStatus);
						}
					}
					catch (Exception exc)
					{
						DBLogger.Instance.ErrorFormat("发生异常，未能操作成功，错误详细：{0}", exc.ToString());

						return Content(exc.ToString());
					}
					finally
					{
						CloseServiceClient(clientR);
					}

					//——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

					/////////////////////////////////////////////////////////////////////////////////////////////////////////////

					DBLogger.Instance.Error("发生异常，未能操作成功");

					return Content("发生异常，未能操作成功");
				}
				//验证失败
				else
				{
					DBLogger.Instance.Error("验证失败");

					return Content("fail");
				}
			}
			else
			{
				DBLogger.Instance.Error("发生异常，未能操作成功");

				return Content("无通知参数");
			}
		}

		[HttpGet]
		public ActionResult Alipay_Callback()
		{
			Dictionary<string, string> sPara = GetRequestGet();

			if (sPara.Count > 0)//判断是否有带返回参数
			{
				Notify aliNotify = new Notify();
				bool verifyResult = aliNotify.VerifyReturn(sPara, Request.QueryString["sign"]);

				if (verifyResult)//验证成功
				{
					/////////////////////////////////////////////////////////////////////////////////////////////////////////////
					//请在这里加上商户的业务逻辑程序代码

					//——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
					//获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

					//商户订单号
					string orderFolio = Request.QueryString["out_trade_no"];

					//支付宝交易号
					string payFolio = Request.QueryString["trade_no"];

					//交易状态
					string result = Request.QueryString["result"];

					//判断是否在商户网站中已经做过了这次通知返回的处理
					//如果没有做过处理，那么执行商户的业务程序
					//如果有做过处理，那么不执行商户的业务程序

					//——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

					var clientR = CreateServiceClient<IRepositoryService>();
					var clientW = CreateServiceClient<IWeChatService>();

					try
					{
						ViewBag.Folio = orderFolio;

						#region "验证参数，状态"
						if (string.IsNullOrEmpty(orderFolio))
						{
							ViewBag.ErrorMsg = "无效订单号：空的订单号";
							DBLogger.Instance.Error("无效订单号：空的订单号");

							return View();
						}

						var uo = clientR.GetUserOrder(orderFolio, true);

						if (uo == null)
						{
							ViewBag.ErrorMsg = string.Format("无效订单号：未找到订单号为‘{0}’的订单", orderFolio);
							DBLogger.Instance.ErrorFormat("无效订单号：未找到订单号为‘{0}’的订单", orderFolio);

							return View();
						}

						var up = clientR.GetPayment(uo.ID);

						if (up == null)
						{
							ViewBag.ErrorMsg = string.Format("无效订单号或支付异常：未找到订单号为‘{0}’的支付记录", orderFolio);
							DBLogger.Instance.ErrorFormat("无效订单号或支付异常：未找到订单号为‘{0}’的支付记录", orderFolio);

							return View();
						}

						if (up.PayStatus != PaymentStatus.Success)
						{
							ViewBag.ErrorMsg = "支付未能成功";
							DBLogger.Instance.ErrorFormat("支付未能成功: {0}", orderFolio);

							return View();
						}
						#endregion

 						if (clientR.ValidatePayment(up.ID, payFolio))
						{
							ViewBag.SuccessMsg = "支付已成功";
							DBLogger.Instance.InfoFormat("支付已成功: {0}", orderFolio);

							// To: Passenger
							clientW.SendMessage(
								uo.RequestorRole == UserRole.Passenger ? uo.Requestor.OpenID : uo.Supplier.OpenID,
								string.Format("{0} 您好，您已成功支付金额 {1} 元，感谢您对绿色环保的支持。请联系车主约好乘车时间和地点！（拼单号：{2}）", uo.RequestorRole == UserRole.Passenger ? uo.Requestor.NickName : uo.Supplier.NickName, up.PayAmount.Value.ToString("C2"), uo.Folio)
							);

							// To: Driver
							clientW.SendMessage(
								uo.RequestorRole == UserRole.Driver ? uo.Requestor.OpenID : uo.Supplier.OpenID,
								string.Format("{0} 您好，乘客 {1} 已经支付金额 {2} 元，感谢您对绿色环保的支持。请联系乘客约好乘车时间和地点！（拼单号：{3}）", uo.RequestorRole == UserRole.Driver ? uo.Requestor.NickName : uo.Supplier.NickName, uo.RequestorRole == UserRole.Passenger ? uo.Requestor.NickName : uo.Supplier.NickName, up.PayAmount.Value.ToString("C2"), uo.Folio)
							);
						}

						return View();
					}
					finally
					{
						CloseServiceClient(clientR);
						CloseServiceClient(clientW);
					}

					/////////////////////////////////////////////////////////////////////////////////////////////////////////////
				}
				else//验证失败
				{
					DBLogger.Instance.Error("验证失败");

					return Content("验证失败");
				}
			}
			else
			{
				DBLogger.Instance.Error("无返回参数");

				return Content("无返回参数");
			}
		}

		#region "Private Methods"
		/// <summary>
		/// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
		/// </summary>
		/// <returns>request回来的信息组成的数组</returns>
		[ValidateInput(false)]
		Dictionary<string, string> GetRequestPost()
		{
			int i = 0;
			Dictionary<string, string> sArray = new Dictionary<string, string>();
			NameValueCollection coll;
			//Load Form variables into NameValueCollection variable.
			coll = Request.Form;

			// Get names of all forms into a string array.
			String[] requestItem = coll.AllKeys;

			for (i = 0; i < requestItem.Length; i++)
			{
				sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
			}

			return sArray;
		}

		/// <summary>
		/// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
		/// </summary>
		/// <returns>request回来的信息组成的数组</returns>
		[ValidateInput(false)]
		Dictionary<string, string> GetRequestGet()
		{
			int i = 0;
			Dictionary<string, string> sArray = new Dictionary<string, string>();
			NameValueCollection coll;
			//Load Form variables into NameValueCollection variable.
			coll = Request.QueryString;

			// Get names of all forms into a string array.
			String[] requestItem = coll.AllKeys;

			for (i = 0; i < requestItem.Length; i++)
			{
				sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
			}

			return sArray;
		} 
		#endregion
		#endregion

		#region "Action: WXPay"
		public ActionResult WXPay(string folio, decimal amount)
		{
			#region "输入参数"
			var dicParameters = new SortedDictionary<string, string>();

			dicParameters.Add("bank_type", "WX");
			dicParameters.Add("body", "");
			dicParameters.Add("attach", folio);
			dicParameters.Add("partner", "");
			dicParameters.Add("out_trade_no", folio);
			dicParameters.Add("total_fee", (amount * 100).ToString());
			dicParameters.Add("fee_type", "1");
			dicParameters.Add("notify_url", string.Format("{0}/payment/wxpay_notify", ConfigStore.CommonSettings.App_Site_Url));
			dicParameters.Add("spbill_create_ip", Request.UserHostAddress);
			dicParameters.Add("input_charset", "UTF-8");
			
			#endregion

			var sbPackages = new StringBuilder();

			foreach (var kv in dicParameters.OrderBy(p => p.Key))
			{
				sbPackages.AppendFormat("{0}={1}&", kv.Key, kv.Value);
			}

			var string1 = sbPackages.ToString().TrimEnd('&');
			var stringSignTemp = string.Format("{0}&key={1}", string1, "");
			var signValue = CommonUtility.GetMd5Str32(string.Format("{0}&key={1}", string1, ""));



			return View();
		}

		public ActionResult WXPay_Notify()
		{
			return View();
		}

		public ActionResult WXPay_Callback()
		{
			return View();
		}
		#endregion
	}
}