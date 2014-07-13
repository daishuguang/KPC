using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.XPath;

using Senparc.Weixin.MP.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.GoogleMap;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.WeChatResponsor
{
    public partial class WeChatHttpResponsor : MessageHandler<MessageContext>
    {
        #region "Events"
        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }

            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();

            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }
        #endregion

        #region "Methods: Service Client"
        protected IContract CreateServiceClient<IContract>()
        {
            var t = typeof(IContract);

            if (t.Equals(typeof(IRepositoryService)))
            {
                return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.Repository_Service_Entry);
            }
            else if (t.Equals(typeof(IRouteMatrixService)))
            {
                return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.RouteMatrix_Service_Entry);
            }
			else if (t.Equals(typeof(IInsuranceService)))
			{
				return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.Insurance_Service_Entry);
			}
            else if (t.Equals(typeof(IWeChatService)))
            {
                return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.WeChat_Service_Entry);
            }
            else if (t.Equals(typeof(ISMSService)))
            {
                return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.SMS_Service_Entry);
            }
            else if (t.Equals(typeof(ILogService)))
            {
                return WCFServiceClientUtility.CreateServiceChanel<IContract>(ConfigStore.APIServiceSettings.Log_Service_Entry);
            }

            return default(IContract);
        }

        protected void CloseServiceClient<IContract>(IContract client)
        {
            WCFServiceClientUtility.CloseServiceChannel(client);
        }
        #endregion

        #region "Events"
        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var client = CreateServiceClient<IRepositoryService>();
            var clientW = CreateServiceClient<IWeChatService>();

            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Archive_WeChatMessage), new { RequestMsg = requestMessage, MsgType = WXMessageType.Text });

                #region "Request：代理"
                if (requestMessage.Content.Trim().StartsWith("@代理") || requestMessage.Content.Trim().StartsWith("代理"))
                {
                    var accessor = client.GetUser(requestMessage.FromUserName);
                    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

                    responseMessage.Content = @"非常感谢，代理专员会尽快和您联系！";

                    return responseMessage;
                }
                #endregion
                #region "Request：反馈"
                else if (requestMessage.Content.Trim().StartsWith("@反馈") || requestMessage.Content.Trim().StartsWith("反馈"))
                {
                    var accessor = client.GetUser(requestMessage.FromUserName);
                    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

                    responseMessage.Content = "非常感谢您的宝贵意见，我们会努力改进！";

                    return responseMessage;
                }
                #endregion
                #region "Request：推荐"
                else if (string.Compare(requestMessage.Content.Trim(), "@推荐", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    string.Compare(requestMessage.Content.Trim(), "推荐", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var accessor = client.GetUser(requestMessage.FromUserName);
                    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

                    if (!accessor.IsRegisterted)
                    {
                        #region "未注册用户"
                        var sb = new StringBuilder();

                        sb.AppendLine("您还没有完成注册哦！只有注册用户才可以开启“推荐”功能！猛戳下面的链接去注册吧！");
                        sb.AppendFormat(@"<a href=""{0}/user/register?returnurl=/user/editprofile"">点我注册</a>", ConfigStore.CommonSettings.App_Site_Url);

                        responseMessage.Content = sb.ToString();
                        #endregion
                    }
                    else
                    {
                        #region "已注册用户"
                        var code = client.GetPromotionCode(accessor.ID, UserPromotionType.User);

                        if (!string.IsNullOrEmpty(code))
                        {
                            var sb = new StringBuilder();

                            sb.AppendLine(@"您的专属推荐码：{0}，让您的朋友关注快拼车(微信号：<a href=""weixin://contacts/profile/kuaipinche"">kuaipinche</a>)，让他们回复您的推荐码：{0} 给快拼车。");
                            sb.Append(@"查询您的推荐统计数据？请直接回复“统计”。");

                            responseMessage.Content = string.Format(sb.ToString(), code);
                        }
                        #endregion
                    }

                    return responseMessage;
                }
                #endregion
                #region "Request：平台合作"
                else if (string.Compare(requestMessage.Content.Trim(), "@平台合作", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    string.Compare(requestMessage.Content.Trim(), "平台合作", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var accessor = client.GetUser(requestMessage.FromUserName);
                    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

                    if (!accessor.IsRegisterted)
                    {
                        #region "未注册用户"
                        var sb = new StringBuilder();

                        sb.AppendLine("您还没有完成注册哦！只有注册用户才可以开启“平台合作”功能！猛戳下面的链接去注册吧！");
						sb.AppendFormat(@"<a href=""{0}/user/register?returnurl=/user/editprofile"">点我注册</a>", ConfigStore.CommonSettings.App_Site_Url);

                        responseMessage.Content = sb.ToString();
                        #endregion
                    }
                    else
                    {
                        #region "已注册用户"
                        var code = client.GetPromotionCode(accessor.ID, UserPromotionType.ExtendChannel);

                        if (!string.IsNullOrEmpty(code))
                        {
                            responseMessage.Content = string.Format("您的平台合作链接为：{0}！查询您的统计数据？请直接回复“统计”！", string.Format("{0}/route/search?channel={1}", ConfigStore.CommonSettings.App_Site_Url, code));
                        }
                        #endregion
                    }

                    return responseMessage;
                }
                #endregion
                #region "Request：统计"
                else if (string.Compare(requestMessage.Content.Trim(), "@统计", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    string.Compare(requestMessage.Content.Trim(), "统计", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var accessor = client.GetUser(requestMessage.FromUserName);
                    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

                    if (!accessor.IsRegisterted)
                    {
                        #region "未注册用户"
                        var sb = new StringBuilder();

                        sb.AppendLine("您还没有完成注册哦！只有注册用户才可以使用“统计”功能！猛戳下面的链接去注册吧！");
						sb.AppendFormat(@"<a href=""{0}/user/register?returnurl=/user/editprofile"">点我注册</a>", ConfigStore.CommonSettings.App_Site_Url);

                        responseMessage.Content = sb.ToString();
                        #endregion
                    }
                    else
                    {
                        #region "已注册用户"
                        var result = client.GetPromotionCount(accessor.ID);

                        if (result != null)
                        {
                            if (result.Status == -1)
                            {
                                #region "未有 推广/代理/渠道 记录的情况"
                                var sb = new StringBuilder();

                                sb.AppendLine("您好，{0}，“快拼车”正在开展以下两种方式的推广活动：");
                                sb.AppendLine("【1】区域代理：您可以直接在微信里以“@代理”开头回复给我们的客服，或直接致电{1}，稍后我们的客服会和您确认后续的事宜。");
                                sb.Append("【2】个人推荐：觉得小拼不错，推荐给您的亲朋好友哟，首先您需要回复“推荐”，小拼会回复一个专属于您的推荐码。只要您的朋友在成功完成注册后，把您得到的“推荐码”回复给小拼，您的推荐就完成啦，稍后更会有奖励信息哦！想实时查询自己的推荐战果？直接回复“统计”哟！");

                                responseMessage.Content = string.Format(sb.ToString(), accessor.NickName, ConfigStore.CommonSettings.Official_Tel);
                                #endregion
                            }
                            else
                            {
                                var sb = new StringBuilder();

                                sb.AppendLine(string.Format("{0}，您好！", accessor.NickName));
                                sb.AppendLine(@"您目前的推荐统计数据如下：");
                                sb.AppendLine(string.Format("总推荐人数：{0}", result.Total_Count));
                                sb.AppendLine(string.Format("其中已注册人数：{0}，加关注人数：{1}，取消关注人数：{2}。", result.Register_Count, result.Subscribe_Count, result.Unsubscribe_Count));

                                #region "推广用户详细"
                                if (result.Total_Count > 0)
                                {
                                    var promUsers = client.LoadPromotionUsers(accessor.ID);

                                    if (promUsers != null)
                                    {
                                        sb.AppendLine();
                                        sb.AppendLine(string.Format("以下列出您近期推荐的 {0} 位用户：", (promUsers.Consumers.Count() >= 10) ? (10) : (promUsers.Consumers.Count())));

                                        var no = 0;

                                        foreach (var promUser in promUsers.Consumers)
                                        {
                                            if (no < promUsers.Consumers.Count() - 1)
                                            {
                                                sb.AppendLine(string.Format("{0:D2}. {1} ({2})", (++no), promUser.NickName, promUser.Status.GetDescription()));
                                            }
                                            else
                                            {
                                                sb.Append(string.Format("{0:D2}. {1} ({2})", (++no), promUser.NickName, promUser.Status.GetDescription()));
                                            }
                                        }
                                    }
                                }
                                #endregion

                                //sb.AppendLine();
                                //sb.AppendFormat(@"<a href=""{0}/promoter/rank"">点击这里</a>，查看推荐排名。", ConfigStore.CommonSettings.App_Site_Url);

                                responseMessage.Content = sb.ToString();
                            }
                        }
                        #endregion
                    }

                    return responseMessage;
                }
                #endregion
                #region "Request: ?/0"
                else if (Regex.IsMatch(requestMessage.Content.Trim(), @"^[\?\？0]$"))
                {
                    return SendMsg_Menu_Help(requestMessage);
                }
                #endregion
                #region "Request: 退出登录"
                else if (string.Compare(requestMessage.Content.Trim(), "退出登录", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

                    responseMessage.Content = string.Format(@"<a href=""{0}/user/signout"">点我退出登录</a>", ConfigStore.CommonSettings.App_Site_Url);

                    return responseMessage;
                }
                #endregion
                else
                {
                    #region "Request: 被推广用户输入推广代码"
                    {
                        var match = Regex.Match(requestMessage.Content.Trim(), @"^\@?(\d{6}|\d{8})$");

                        if (match.Success)
                        {
                            var accessor = client.GetUser(requestMessage.FromUserName);
                            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                            var code = 0;

                            if (int.TryParse(match.Groups[1].Value, out code))
                            {
                                #region "更新推广计数"
                                var result = client.SetPromotionUser(accessor.ID, code);

                                if (result != null)
                                {
                                    if (result.Status != 0)
                                    {
                                        #region "错误推荐码/重复推荐码"
                                        switch (result.Status)
                                        {
                                            case -1:
                                                responseMessage.Content = "您已经成功被推荐，不可以再次被别人推荐哦~";
                                                break;
                                            case -2:
                                            default:
                                                responseMessage.Content = "您输入的推荐码好像不对哦，请确认后重新输入！";
                                                break;
                                        }

                                        return responseMessage;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region "成功输入推荐码"
                                        var sb = new StringBuilder();

                                        sb.AppendLine(string.Format("恭喜 {0}，您已经成功地被 {1} 推荐为快拼车用户！", accessor.NickName, result.NickName));

                                        if (!accessor.IsRegisterted)
                                        {
                                            sb.AppendLine();
											sb.AppendLine(string.Format(@"就差最后一步，即可抽取iphone 5s, ipad mini及现金大奖，<a href=""{0}/user/register?returnurl=/user/editprofile"">点我免费注册吧</a>！", ConfigStore.CommonSettings.App_Site_Url));

                                            if (client.CheckIsSubscribeFirst(accessor.ID))
                                            {
                                                // 向推荐人发送微信通知消息
                                                clientW.SendMessage(result.OpenID, string.Format("{0} 您好，您推荐的用户 {1} 已成功首次加关注快拼车公众账号！", result.NickName, accessor.NickName), false);
                                            }
                                        }
                                        else
                                        {
                                            sb.AppendLine();
                                            sb.AppendFormat("此外您现在也可以通过回复“推荐”开启专属于您的个人推广功能啦！详情您可以咨询我们的客服，或直接致电{0}咨询详细事宜。", ConfigStore.CommonSettings.Official_Tel);

                                            if (client.CheckIsRegisterFirst(accessor.ID))
                                            {
                                                // 向推荐人发送微信通知消息
                                                clientW.SendMessage(result.OpenID, string.Format("{0} 您好，您推荐的用户 {1} 已成功首次注册快拼车公众账号！", result.NickName, accessor.NickName), false);
                                            }
                                        }

                                        responseMessage.Content = sb.ToString();

                                        return responseMessage;
                                        #endregion
                                    }
                                }
                                #endregion
                            }

                            return null;
                        }
                    }
                    #endregion

                    #region "Request: Number"
                    {
                        var match = Regex.Match(requestMessage.Content.Trim(), @"^([1-9]|10|11|99)$");

                        if (match.Success)
                        {
                            return SendMsg_IdentityKey_Number(requestMessage, int.Parse(match.Groups[0].Value), client);
                        }
                    }
                    #endregion

                    #region "Request: 其它非命令字符"
					//if (IsOutOfWorkTime() || !WeChatCustomerServiceManager.Instance.IsExist(requestMessage.FromUserName))
					//{
					//	return SendMsg_Menu(requestMessage, client);
					//}
					//else
					//{
					//	WeChatCustomerServiceManager.Instance.Add(requestMessage.FromUserName);
					//}

					//return null;

					return SendMsg_Menu(requestMessage, client);
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
												"IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)",
												requestMessage.FromUserName,
												requestMessage.ToUserName,
												string.Empty,
												requestMessage.MsgId,
												requestMessage.MsgType,
												ex.ToString())
					}
				);
            }
            finally
            {
                CloseServiceClient(client);
                CloseServiceClient(clientW);
            }

            return null;
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Archive_WeChatMessage), new { RequestMsg = requestMessage, MsgType = WXMessageType.Location });

                var accessor = client.GetUser(requestMessage.FromUserName);

                if (accessor != null)
                {
                    // Update User Track with new position data
                    client.SetUserTrack(accessor.UserGUID, Point.Convert(new Point { Longitude = requestMessage.Location_Y, Latitude = requestMessage.Location_X }, PointCoordType.GoogleMap_gcj0211, PointCoordType.BaiduMap_bd0911));

                    return SendMsg_Menu_Around(requestMessage, client);
                }
            }
            catch (Exception ex)
            {
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("Method: {0}\r\nFromUser: {1}, ToUser: {2}, EventKey: {3}, MsgID: {4}, MsgType: {5}\r\nException: {6}",
												"IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)",
												requestMessage.FromUserName,
												requestMessage.ToUserName,
												string.Empty,
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
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            return SendMsg_Menu(requestMessage);
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            return SendMsg_Menu(requestMessage);
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            return SendMsg_Menu(requestMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

            //responseMessage.Content = "这条消息来自DefaultResponseMessage。";

            //return responseMessage;

            return null;
        }
        #endregion


        #region "Private Methods: SendMsg_IdentityKey_Number"
        IResponseMessageBase SendMsg_IdentityKey_Number(RequestMessageText requestMessage, int num, IRepositoryService client)
        {
            switch (num)
            {
                case 1:
                    return SendMsg_Menu_Create(requestMessage, client);
                case 2:
                    return SendMsg_Menu_Search(requestMessage, client);
                case 3:
                    return SendMsg_Menu_Plaza(requestMessage, client);
                case 4:
                    return SendMsg_Menu_Around(requestMessage, client);
                case 5:
                    return SendMsg_Menu_MyRoute(requestMessage, client);
                case 6:
                    return SendMsg_Menu_MyOrder(requestMessage, client);
                case 7:
                    return SendMsg_Menu_MyProfile(requestMessage, client);
                case 8:
                    return SendMsg_Menu_More(requestMessage, client);
                case 9:
                    return SendMsg_Menu_Agent(requestMessage, client);
                case 10:
                    return SendMsg_Menu_Feedback(requestMessage, client);
                case 11:
					return SendMsg_Menu_History(requestMessage, client);
                case 99:
                    return SendMsg_Menu_CustomService(requestMessage, client);
                default:
                    if (ConfigStore.CommonSettings.Trace_Mode)
                    {
						DBLogger.Instance.Error(new Utils.Logger.Base.WXLogMessage { UserID = requestMessage.FromUserName, Message = string.Format("Invalid IdentityKey Number: {0}", num) });
                    }
                    return null;
            }
        }

        bool IsOutOfWorkTime()
        {
            return (DateTime.Now.Hour >= 22 || DateTime.Now.Hour < 9);
        }
        #endregion

        #region "Private Methods: SendMsg_Menu"
        IResponseMessageBase SendMsg_Menu<IT>(IT requestMessage, IRepositoryService client = null)
            where IT : IRequestMessageBase
        {
            #region "菜单"
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            var msg = new StringBuilder();

            if (client != null)
            {
                var accessor = client.GetUser(requestMessage.FromUserName);

                if (accessor.Status != UserStatus.Registered)
                {
					msg.AppendLine(string.Format(@"就差最后一步，即可抽取iphone 5s, ipad mini及现金大奖，<a href=""{0}/user/register?returnurl=/user/editprofile"">点我免费注册吧</a>！", ConfigStore.CommonSettings.App_Site_Url));
                    msg.AppendLine();
                }
            }

            msg.AppendLine(@"请回复序号：");
            msg.AppendLine(@"【0】帮助菜单");
            msg.AppendLine(string.Format(@"【1】<a href=""{0}/route/create"">发布路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【2】<a href=""{0}/route/search"">搜索路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【3】<a href=""{0}/route/plaza"">最新路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【4】<a href=""{0}/user/aroundlist"">附近拼友</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【5】<a href=""{0}/route/list"">我的路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【6】<a href=""{0}/order/list"">我的拼单</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【7】<a href=""{0}/user/editprofile"">我的资料</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(@"【8】查看更多");
            msg.AppendLine();
            msg.Append(@"人工服务，请回复【99】");

            responseMessage.Content = msg.ToString();

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_Subscribe<IT>(IT requestMessage)
            where IT : IRequestMessageBase
        {
            #region "菜单"
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            var msg = new StringBuilder();

            msg.AppendLine(@"欢迎关注快拼车，国内最大微信拼车平台！");
			msg.AppendLine(string.Format(@"就差最后一步，即可抽取iphone 5s, ipad mini及现金大奖，<a href=""{0}/user/register?returnurl=/user/editprofile"">点我免费注册吧</a>！", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine();
            msg.AppendLine(@"请回复序号：");
            msg.AppendLine(@"【0】帮助菜单");
            msg.AppendLine(string.Format(@"【1】<a href=""{0}/route/create"">发布路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【2】<a href=""{0}/route/search"">搜索路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【3】<a href=""{0}/route/plaza"">最新路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【4】<a href=""{0}/user/aroundlist"">附近拼友</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【5】<a href=""{0}/route/list"">我的路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【6】<a href=""{0}/order/list"">我的拼单</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【7】<a href=""{0}/user/editprofile"">我的资料</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(@"【8】查看更多");
            msg.AppendLine();
            msg.Append(@"人工服务，请回复【99】");

            responseMessage.Content = msg.ToString();

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_Help<IT>(IT requestMessage)
            where IT : IRequestMessageBase
        {
            #region "菜单"
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            var msg = new StringBuilder();

            msg.AppendLine(@"请回复序号：");
            msg.AppendLine(@"【0】帮助菜单");
            msg.AppendLine(string.Format(@"【1】<a href=""{0}/route/create"">发布路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【2】<a href=""{0}/route/search"">搜索路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【3】<a href=""{0}/route/plaza"">最新路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【4】<a href=""{0}/user/aroundlist"">附近拼友</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【5】<a href=""{0}/route/list"">我的路线</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【6】<a href=""{0}/order/list"">我的拼单</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(string.Format(@"【7】<a href=""{0}/user/editprofile"">我的资料</a>", ConfigStore.CommonSettings.App_Site_Url));
            msg.AppendLine(@"【8】查看更多");
            msg.AppendLine();
            msg.Append(@"人工服务，请回复【99】");

            responseMessage.Content = msg.ToString();

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_Plaza<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
		{
			#region "菜单：最新路线"
			var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

			responseMessage.Content = "正在为您查询，请稍后。。。";

			// 发送异步数据消息
			ThreadPool.QueueUserWorkItem(new WaitCallback(SendMsg_Data_Async_Plaza), requestMessage);

			return responseMessage;
			#endregion
        }

        IResponseMessageBase SendMsg_Menu_Create<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "菜单：发布路线"
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();

            responseMessage.Articles.Add(new Article
            {
                Title = "发布路线"
            });

            var sb = new StringBuilder();

            sb.AppendLine("想找到和您顺路和乘客或司机么？发布一条上下班路线或长途路线吧，让对方轻松找到您。");
            sb.AppendLine();
            sb.Append("点击进入");

            responseMessage.Articles.Add(new Article
            {
                Title = sb.ToString(),
                Url = string.Format("{0}/route/create", ConfigStore.CommonSettings.App_Site_Url)
            });

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_Search<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "菜单：搜索路线"
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();

            responseMessage.Articles.Add(new Article
            {
                Title = "搜索路线"
            });

            var sb = new StringBuilder();

            sb.AppendLine("想找到和您顺路和乘客或司机么？输入您的起始地点，搜索一下吧。");
            sb.AppendLine();
            sb.Append("点击进入");

            responseMessage.Articles.Add(new Article
            {
                Title = sb.ToString(),
                Url = string.Format("{0}/route/search", ConfigStore.CommonSettings.App_Site_Url)
            });

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_Around<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "菜单：附近拼友"
			var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

			responseMessage.Content = "正在为您查询，请稍后。。。";

			// 发送异步数据消息
			ThreadPool.QueueUserWorkItem(new WaitCallback(SendMsg_Data_Async_Around), requestMessage);

			return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_MyOrder<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "菜单：我的拼单"
			var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

			responseMessage.Content = "正在为您查询，请稍后。。。";

			// 发送异步数据消息
			ThreadPool.QueueUserWorkItem(new WaitCallback(SendMsg_Data_Async_MyOrder), requestMessage);

			return responseMessage;
			#endregion
        }

        IResponseMessageBase SendMsg_Menu_MyRoute<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
			#region "菜单：我的路线"
			var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

			responseMessage.Content = "正在为您查询，请稍后。。。";

			// 发送异步数据消息
			ThreadPool.QueueUserWorkItem(new WaitCallback(SendMsg_Data_Async_MyRoute), requestMessage);

			return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_MyProfile<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "菜单：我的资料"
            var accessor = client.GetUser(requestMessage.FromUserName);
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();

            responseMessage.Articles.Add(new Article
            {
                Title = "我的资料"
            });

            if (accessor.IsRegisterted)
            {
                var sb = new StringBuilder();

                sb.AppendLine(string.Format("昵　称：{0}", accessor.NickName));
                sb.AppendLine(string.Format("手机号：{0}", accessor.Mobile));
                sb.AppendLine(string.Format("身　份：{0}", accessor.UserRole.GetDescription()));
                sb.AppendLine();
                sb.Append("点击进入");

                responseMessage.Articles.Add(new Article
                {
                    Title = sb.ToString(),
                    Url = string.Format("{0}/user/editprofile", ConfigStore.CommonSettings.App_Site_Url)
                });
            }
            else
            {
                responseMessage.Articles.Add(new Article
                {
                    Title = "您还没有注册哦，完成注册之后别人才能联系到您哦。",
                    Url = string.Format("{0}/user/editprofile", ConfigStore.CommonSettings.App_Site_Url)
                });
            }

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_More<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "菜单：查看更多"
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            var msg = new StringBuilder();

            msg.AppendLine(@"请回复序号：");
            msg.AppendLine(@"【9】区域代理");
			msg.AppendLine(@"【10】联系反馈");
            msg.AppendLine(@"【11】历史版本");
            msg.AppendLine(@"【0】帮助菜单");
            msg.AppendLine();
            msg.Append(@"人工服务，请回复【99】");

            responseMessage.Content = msg.ToString();

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_Agent<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "菜单：区域代理"
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            var msg = new StringBuilder();

            msg.Append(@"快拼车区域代理火热招募中，如果您有车辆、乘客、媒体等资源或推广的激情，请以“代理”开头，将您的简介和联系方式回复我们。");

            responseMessage.Content = msg.ToString();

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_Feedback<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
		{
			#region "菜单：联系反馈"
			var responseMessage = CreateResponseMessage<ResponseMessageText>();
            var msg = new StringBuilder();

            msg.AppendLine(@"请以“反馈”开头，将您的吐槽回复我们，我们会抽取用户送出精美礼品。您还可以通过以下方式联系到我们：");
            msg.AppendLine();
            msg.AppendLine("如需人工服务，请回复序号【99】");
            msg.AppendLine(string.Format(@"客服热线：{0}", ConfigStore.CommonSettings.Official_Tel));
            msg.AppendLine(string.Format(@"客服邮箱：{0}", ConfigStore.CommonSettings.Official_Email));
            msg.AppendLine(string.Format(@"新浪微博：<a href=""{0}"">@快拼车</a>", ConfigStore.CommonSettings.Official_Weibo));
            msg.Append(string.Format(@"客服QQ：{0}", ConfigStore.CommonSettings.CustomerService_Supervisor_QQ));

            responseMessage.Content = msg.ToString();

            return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_History<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
		{
			#region "菜单：历史版本"
			var responseMessage = CreateResponseMessage<ResponseMessageNews>();

			responseMessage.Articles.Add(new Article
			{
				Title = "历史版本"
			});

			var sb = new StringBuilder();

			sb.AppendLine("见证快拼车的一步步成长，查看历史版本，看看更新了什么。");
			sb.AppendLine();
			sb.Append("点击进入");

			responseMessage.Articles.Add(new Article
			{
				Title = sb.ToString(),
				Url = string.Format("{0}/whatsnew", ConfigStore.CommonSettings.App_Site_Url)
			});

			return responseMessage;
            #endregion
        }

        IResponseMessageBase SendMsg_Menu_CustomService<IT>(IT requestMessage, IRepositoryService client)
            where IT : IRequestMessageBase
        {
            #region "99: 转接客服"
			if (IsOutOfWorkTime())
			{
				var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

				responseMessage.Content = "小拼的工作时间是 09:00 - 21:00，如需拼车请留下您的个人信息：司机/乘客、起点、终点、时间和手机号码。谢谢！";

				return responseMessage;
			}
			else
			{
				var clientW = CreateServiceClient<IWeChatService>();

				try
				{
					var accessor = client.GetUser(requestMessage.FromUserName);

					#region "发送前导信息给用户"
					clientW.SendMessage(accessor.OpenID, "将为您转接客服专员，请稍等 。。。 请简单描述您的问题，以便我们的客服人员可以快速帮到您。谢谢！", false); 
					#endregion

					#region "发送提醒信息给客服"
					var openIDs = ConfigStore.CommonSettings.CustomerService_Supervisor_OpenID.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

					if (openIDs != null && openIDs.Length > 0)
					{
						foreach (var openID in openIDs)
						{
							clientW.SendMessage(openID, string.Format("快拼车客服，有客户 {0} 来求助了，赶快登录公众平台去看看！", accessor.NickName), false);
						}
					} 
					#endregion
				}
				finally
				{
					CloseServiceClient(clientW);
				}

				// 加入客服模式队列
				//WeChatCustomerServiceManager.Instance.Add(requestMessage.FromUserName);

				// 开启多客服模式
				return ResponseMessageBase.CreateFromRequestMessage<ResponseMessageTransfer_Customer_Service>(requestMessage);
			}
			#endregion
        }
        #endregion

		#region "Private Methods: SendMsg_Data_Async"
		void SendMsg_Data_Async_Plaza(object state)
		{
			#region "菜单：最新路线"
			var clientR = CreateServiceClient<IRepositoryService>();
			var requestMessage = state as IRequestMessageBase;

			try
			{
				var accessor = clientR.GetUser(requestMessage.FromUserName);
				var userTrack = clientR.GetUserTrack(accessor.ID, true);
				var city = string.Empty;

				#region "获取当前所在城市"
				if (userTrack != null)
				{
					if (userTrack.Route != null)
					{
						city = userTrack.Route.From_City;
					}
					else if (userTrack.Position != null && userTrack.Position.HasValue)
					{
						var loc = Location.Reverse(userTrack.Position.Value);

						if (loc != null)
						{
							city = loc.City;
						}
					}
				}
				#endregion

				var results = clientR.LoadUserRouteNewest(new RouteSearchRequest { Filter = new RouteSearchFilter { LocationFilter = new RouteSearch_LocationFilter { From_City = city } }, Page = 0, Count = 10 }, true);
				var sbContent = default(StringBuilder);

				if (results != null && results.Count() > 0)
				{
					var responseMessage = CreateResponseMessage<ResponseMessageNews>();

					#region "有路线状态"
					responseMessage.Articles.Add(new Article
					{
						Title = "最新路线"
					});

					var uIDs = new List<Guid>();
					var count = 0;
					var page = 0;

					do
					{
						foreach (var result in results)
						{
							if (!uIDs.Contains(result.User.UserGUID))
							{
								uIDs.Add(result.User.UserGUID);
								count++;

								#region "路线简要信息"
								sbContent = new StringBuilder();

								sbContent.AppendLine(string.Format("起点：{0}", GetRouteLocationDescription(result.Route, RouteDirection.From)));
								sbContent.AppendLine(string.Format("终点：{0}", GetRouteLocationDescription(result.Route, RouteDirection.To)));
								sbContent.Append(GetRouteTypeDescription(result.Route));

								responseMessage.Articles.Add(new Article
								{
									Title = sbContent.ToString(),
									PicUrl = string.Format("{0}/{1}", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'), (!string.IsNullOrEmpty(result.User.PortraitsThumbUrl)) ? (result.User.PortraitsThumbUrl.TrimStart('/')) : (ConfigStore.CommonSettings.Portraits_ImageDefault)),
									Url = string.Format("{0}/route/view?id={1}&from_lng=0&from_lat=0&to_lng=0&to_lat=0#mp.weixin.qq.com", ConfigStore.CommonSettings.App_Site_Url, result.Route.RouteGUID)
								});
								#endregion
							}

							if (count == 5)
							{
								break;
							}
						}

						if (count < 5)
						{
							page++;
							results = clientR.LoadUserRouteNewest(new RouteSearchRequest { Filter = new RouteSearchFilter { LocationFilter = new RouteSearch_LocationFilter { From_City = city } }, Page = page, Count = 10 });
						}
					} while (count < 5 && results != null && results.Count() > 0);

					responseMessage.Articles.Add(new Article
					{
						Title = "查看更多 >>",
						Url = string.Format("{0}/route/plaza#w", ConfigStore.CommonSettings.App_Site_Url)
					});
					#endregion

					SendMsg_Data_Async(responseMessage);
				}
				else
				{
					var responseMessage = CreateResponseMessage<ResponseMessageText>();

					#region "无路线状态"
					sbContent = new StringBuilder();
					sbContent.AppendFormat(@"系统还未收录任何路线哦，赶紧去点击<a href=""{0}/route/create"">“发布路线”</a>，或者点屏幕下方的菜单：“我要拼车”->“发布路线”，开始您的拼车生活吧！", ConfigStore.CommonSettings.App_Site_Url);

					responseMessage.Content = sbContent.ToString();
					#endregion

					SendMsg_Data_Async(responseMessage);
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{ 
						UserID = requestMessage.FromUserName,
						Message = string.Format("回复客户异步消息错误：Method: {0}, Data: [{1}], Exception: {2}", "SendMsg_Data_async_Plaza", (requestMessage != null ? (string.Format("FromUserName: {0}, ToUserName: {1}, MsgType: {2}, Createdate: {3}", requestMessage.FromUserName, requestMessage.ToUserName, requestMessage.MsgType, requestMessage.CreateTime)) : ("NULL")), ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(clientR);
			} 
			#endregion
		}

		void SendMsg_Data_Async_Around(object state)
		{
			#region "菜单：附近拼友"
			var clientR = CreateServiceClient<IRepositoryService>();
			var requestMessage = state as IRequestMessageBase;

			try
			{
				var accessor = clientR.GetUser(requestMessage.FromUserName);
				var userTrack = clientR.GetUserTrack(accessor.ID, true);

				if (userTrack != null)
				{
					var responseMessage = CreateResponseMessage<ResponseMessageNews>();

					#region "构造菜单数据"
					responseMessage.Articles.Add(new Article
					{
						Title = "附近拼友（发送位置更新拼友信息）"
					});

					var uts = clientR.LoadUserAround(new UserTrackRequestList
					{
						Filter = new UserTrackRequest.UserTrackRequestFilter { UserID = accessor.UserGUID },
						Count = 5
					});

					if (uts != null)
					{
						foreach (var ut in uts)
						{
							#region "拼友简要信息"
							var sbContent = new StringBuilder();

							sbContent.AppendLine(string.Format("{0}  {1}", ut.User.NickName, Libs.Utils.CommonUtility.FormatDistance(ut.Distance)));

							if (ut.Route != null)
							{
								sbContent.AppendLine(string.Format("起点：{0}", GetRouteLocationDescription(ut.Route, RouteDirection.From)));
								sbContent.AppendLine(string.Format("终点：{0}", GetRouteLocationDescription(ut.Route, RouteDirection.To)));
								sbContent.Append(GetRouteTypeDescription(ut.Route));
							}
							else
							{
								sbContent.Append("（暂时未发布路线）");
							}

							responseMessage.Articles.Add(new Article
							{
								Title = sbContent.ToString(),
								PicUrl = string.Format("{0}/{1}", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'), (!string.IsNullOrEmpty(ut.User.PortraitsThumbUrl)) ? (ut.User.PortraitsThumbUrl.TrimStart('/')) : (string.Empty)),
								Url = string.Format("{0}/user/viewprofile/{1}#w", ConfigStore.CommonSettings.App_Site_Url, ut.User.UserGUID)
							});
							#endregion
						}
					}

					responseMessage.Articles.Add(new Article
					{
						Title = "查看更多 >>",
						Url = string.Format("{0}/user/aroundlist#w", ConfigStore.CommonSettings.App_Site_Url)
					});
					#endregion

					SendMsg_Data_Async(responseMessage);
				}
				else
				{
					var responseMessage = CreateResponseMessage<ResponseMessageText>();

					responseMessage.Content = "请点击屏幕左下角的键盘图标，然后点击【+】图标，选择【位置】，最后点击【发送】按钮提交您的地理位置。";

					SendMsg_Data_Async(responseMessage);
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("回复客户异步消息错误：Method: {0}, Data: [{1}], Exception: {2}", "SendMsg_Data_Async_Around", (requestMessage != null ? (string.Format("FromUserName: {0}, ToUserName: {1}, MsgType: {2}, Createdate: {3}", requestMessage.FromUserName, requestMessage.ToUserName, requestMessage.MsgType, requestMessage.CreateTime)) : ("NULL")), ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(clientR);
			} 
			#endregion
		}

		void SendMsg_Data_Async_MyOrder(object state)
		{
			#region "菜单：我的拼单"
			var clientR = CreateServiceClient<IRepositoryService>();
			var requestMessage = state as IRequestMessageBase;

			try
			{
				var accessor = clientR.GetUser(requestMessage.FromUserName);
				var results = clientR.GetUserOrder(new UserOrderRequest { Filter = new UserOrderRequest.UserOrderRequestFilter { UserID = accessor.UserGUID, OrderType = OrderType.All }, Page = 0, Count = 5 }, true);
				var sbContent = default(StringBuilder);

				if (results != null && results.Count() > 0)
				{
					var responseMessage = CreateResponseMessage<ResponseMessageNews>();

					#region "有拼单状态"
					responseMessage.Articles.Add(new Article
					{
						Title = "我的拼单"
					});

					foreach (var result in results)
					{
						#region "拼单简要信息"
						sbContent = new StringBuilder();

						sbContent.AppendLine(string.Format("起点：{0}", GetRouteLocationDescription(result.Route, RouteDirection.From)));
						sbContent.AppendLine(string.Format("终点：{0}", GetRouteLocationDescription(result.Route, RouteDirection.To)));
						sbContent.AppendLine(string.Format("时间：{0}", result.StartDate.Value.ToString("MM月dd日 HH:mm 出发")));
						sbContent.Append(string.Format("状态：{0}", result.Status.GetDescription()));

						var picUrl = string.Empty;

						if (accessor.UserGUID.CompareTo(result.Requestor.UserGUID) == 0)
						{
							// 请求者的情况
							picUrl = result.Supplier.PortraitsThumbUrl;
						}
						else
						{
							// 线路所有者的情况
							picUrl = result.Requestor.PortraitsThumbUrl;
						}

						responseMessage.Articles.Add(new Article
						{
							Title = sbContent.ToString(),
							PicUrl = string.Format("{0}/{1}", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'), (!string.IsNullOrEmpty(picUrl)) ? (picUrl.TrimStart('/')) : (ConfigStore.CommonSettings.Portraits_ImageDefault)),
							Url = string.Format("{0}/order/detail/{1}", ConfigStore.CommonSettings.App_Site_Url, result.Folio)
						});
						#endregion
					}

					responseMessage.Articles.Add(new Article
					{
						Title = "查看更多 >>",
						Url = string.Format("{0}/order/list#w", ConfigStore.CommonSettings.App_Site_Url, (int)OrderType.AssignToMe)
					});
					#endregion

					SendMsg_Data_Async(responseMessage);
				}
				else
				{
					var responseMessage = CreateResponseMessage<ResponseMessageText>();

					#region "无拼单的状态"
					sbContent = new StringBuilder();
					sbContent.AppendFormat(@"您还没有收到过任何拼单哦，赶紧去点击<a href=""{0}/route/create"">“发布路线”</a>，或者点屏幕下方的菜单：“我要拼车”->“发布路线”，开始您的拼车生活吧！", ConfigStore.CommonSettings.App_Site_Url);

					responseMessage.Content = sbContent.ToString();
					#endregion

					SendMsg_Data_Async(responseMessage);
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("回复客户异步消息错误：Method: {0}, Data: [{1}], Exception: {2}", "SendMsg_Data_Async_MyOrder", (requestMessage != null ? (string.Format("FromUserName: {0}, ToUserName: {1}, MsgType: {2}, Createdate: {3}", requestMessage.FromUserName, requestMessage.ToUserName, requestMessage.MsgType, requestMessage.CreateTime)) : ("NULL")), ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(clientR);
			} 
			#endregion
		}

		void SendMsg_Data_Async_MyRoute(object state)
		{
			#region "菜单：我的路线"
			var clientR = CreateServiceClient<IRepositoryService>();
			var requestMessage = state as IRequestMessageBase;

			try
			{
				var accessor = clientR.GetUser(requestMessage.FromUserName);
				var results = clientR.LoadUserRouteByUserID(accessor.ID, 0, 5, true);
				var sbContent = default(StringBuilder);

				if (results != null && results.Count() > 0)
				{
					var responseMessage = CreateResponseMessage<ResponseMessageNews>();

					#region "有路线状态"
					responseMessage.Articles.Add(new Article
					{
						Title = "我的路线"
					});

					foreach (var result in results)
					{
						#region "路线简要信息"
						sbContent = new StringBuilder();

						sbContent.AppendLine(string.Format("起点：{0}", GetRouteLocationDescription(result.Route, RouteDirection.From)));
						sbContent.AppendLine(string.Format("终点：{0}", GetRouteLocationDescription(result.Route, RouteDirection.To)));
						sbContent.Append(GetRouteTypeDescription(result.Route));

						responseMessage.Articles.Add(new Article
						{
							Title = sbContent.ToString(),
							PicUrl = string.Format("{0}/{1}", ConfigStore.CommonSettings.App_Site_Url.TrimEnd('/'), (!string.IsNullOrEmpty(result.User.PortraitsThumbUrl)) ? (result.User.PortraitsThumbUrl.TrimStart('/')) : (ConfigStore.CommonSettings.Portraits_ImageDefault)),
							Url = string.Format("{0}/route/detail/{1}", ConfigStore.CommonSettings.App_Site_Url, result.Route.RouteGUID)
						});
						#endregion
					}

					responseMessage.Articles.Add(new Article
					{
						Title = "查看更多 >>",
						Url = string.Format("{0}/route/list#w", ConfigStore.CommonSettings.App_Site_Url)
					});
					#endregion

					SendMsg_Data_Async(responseMessage);
				}
				else
				{
					var responseMessage = CreateResponseMessage<ResponseMessageText>();

					#region "无路线的状态"
					sbContent = new StringBuilder();
					sbContent.AppendFormat(@"您还没有发布过任何路线哦，赶紧去尝试一下吧，点击<a href=""{0}/route/create"">“发布路线”</a>，或点击屏幕下方的菜单：“我要拼车”->“发布路线”。", ConfigStore.CommonSettings.App_Site_Url);

					responseMessage.Content = sbContent.ToString();
					#endregion

					SendMsg_Data_Async(responseMessage);
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.Error(
					new Utils.Logger.Base.WXLogMessage
					{
						UserID = requestMessage.FromUserName,
						Message = string.Format("回复客户异步消息错误：Method: {0}, Data: [{1}], Exception: {2}", "SendMsg_Data_Async_MyRoute", (requestMessage != null ? (string.Format("FromUserName: {0}, ToUserName: {1}, MsgType: {2}, Createdate: {3}", requestMessage.FromUserName, requestMessage.ToUserName, requestMessage.MsgType, requestMessage.CreateTime)) : ("NULL")), ex.ToString())
					}
				);
			}
			finally
			{
				CloseServiceClient(clientR);
			}
			#endregion
		}
		#endregion

		#region "Private Methods"
		string GetRouteTypeDescription(Route route)
        {
            var sbContent = new StringBuilder();

            switch (route.RouteType)
            {
                case RouteType.Citywide_EveryDay:
                    sbContent.Append(route.StartDate.Value.ToString("每天 HH:mm 出发"));
                    break;
                case RouteType.Citywide_Temp:
                    sbContent.Append(route.StartDate.Value.ToString("MM月dd日 HH:mm 出发"));
                    break;
                case RouteType.Citywide_Weekend:
                    sbContent.Append(route.StartDate.Value.ToString("每周末 HH:mm 出发"));
                    break;
                case RouteType.Citywide_Workday:
                    sbContent.Append(route.StartDate.Value.ToString("工作日 HH:mm 出发"));
                    break;
                case RouteType.Intercity_EveryDay:
                    sbContent.Append("长期有效");
                    break;
                case RouteType.Intercity_Weekend:
                    sbContent.Append("每周末出发");
                    break;
                case RouteType.Intercity_Workday:
                    sbContent.Append("每工作日出发");
                    break;
                case RouteType.Intercity_Temp:
                    sbContent.Append(route.StartDate.Value.ToString("MM月dd日 出发"));
                    break;
            }

            return sbContent.ToString();
        }

		string GetRouteLocationDescription(Route route, RouteDirection direction)
		{
			var content = string.Empty;
			var isCitywide = (((int)route.RouteType & 0x10) != 0);

			switch (direction)
			{
				case RouteDirection.From:
					content = isCitywide ? string.Format("{0}{1}", route.From_District, route.From_Location) : string.Format("{0}{1}", route.From_City, route.From_Location);
					break;
				case RouteDirection.To:
					content = isCitywide ? string.Format("{0}{1}", route.To_District, route.To_Location) : string.Format("{0}{1}", route.To_City, route.To_Location);
					break;
			}

			return content;
		}

		void SendMsg_Data_Async<T>(T responseMessage)
			where T : IResponseMessageBase
		{
			var clientW = CreateServiceClient<IWeChatService>();

			try
			{
				if (responseMessage == null)
				{
					throw new Exception("Fail to execute methods 'SendMsg_Data_Async' with null reference of object 'responseMessage'!");
				}

				var t = responseMessage.GetType();
				
				if (responseMessage is ResponseMessageText)
				{
					clientW.SendMessage(new CustomerMessage_Text(responseMessage.ToUserName, (responseMessage as ResponseMessageText).Content));
				}
				else if (responseMessage is ResponseMessageNews)
				{
					var msg = new CustomerMessage_News() { ToUser = responseMessage.ToUserName };

					foreach (var article in (responseMessage as ResponseMessageNews).Articles)
					{
						msg.News.Articles.Add(new MessageContent_News_Article { Title = article.Title, Description = article.Description, Url = article.Url, PicUrl = article.PicUrl });
					}

					clientW.SendMessage(msg);
				}
			}
			finally
			{
				CloseServiceClient(clientW);
			}
		}
        #endregion
    }
}
