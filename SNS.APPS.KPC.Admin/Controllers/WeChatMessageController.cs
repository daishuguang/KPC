using Senparc.Weixin.MP;
using SNS.Apps.KPC.Admin.CustomAttribute;
using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.Models;
using SNS.Apps.KPC.Admin.Services;
using SNS.Apps.KPC.Admin.Utilities;
using SNS.Apps.KPC.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Admin.Controllers
{
    [CustomAuthorizeAttribute]
    public class WeChatMessageController : BaseController
    {
        public IWeChatMessageServices WeChatMessageServices;

        public WeChatMessageController()
        {
            WeChatMessageServices = new WeChatMessageServices();
        }

        //
        // GET: /WeChatMessage/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetWeChatMessagesData(int page = 1, int rows = 10, string sort = "CreateDate", string order = "desc")
        {
            int total = 0;
            var messages = WeChatMessageServices.GetMessages(page, rows, sort, order, out total);
            IList<WeChatMessageDetailsViewModel> modelList = new List<WeChatMessageDetailsViewModel>();
            foreach (var item in messages)
            {
                string msgRenderContent = RequestMsgHelper.CreateMessageContentRenderString(item.MsgContent);
                WeChatMessageDetailsViewModel model = new WeChatMessageDetailsViewModel
                {
                    ID = item.ID,
                    From_OpenID = item.From_OpenID,
                    MsgContent = item.MsgContent,
                    MsgID = item.MsgID,
                    MsgType = item.MsgType,
                    MsgRenderContent = msgRenderContent,
                    CreateDate = item.CreateDate,
                    To_OpenID = item.To_OpenID
                };
                modelList.Add(model);
            }
            SNSEntityJsonViewModel<WeChatMessageDetailsViewModel> viewModel = new SNSEntityJsonViewModel<WeChatMessageDetailsViewModel>();
            viewModel.rows = modelList;
            viewModel.total = total;
            WeChatMessageServices.Dispose();
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult WeChatMessageDetails(string search, int page = 1, int rows = 10, string sort = "CreateDate", string order = "desc")
        {
            int total = 0;
            Dictionary<string, object> searchDir = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(search))
            {
                searchDir.Add(search.Split(':')[0], search.Split(':')[1]);
            }
            var messages = WeChatMessageServices.GetMessagesContainsUser(page, rows, sort, order, searchDir, out total);
            SNSEntityJsonViewModel<WeChatMessageAndUserViewModel> viewModel = BindMessageViewModel(messages.ToList(), total);
            WeChatMessageServices.Dispose();
            return PartialView(viewModel);
        }

        SNSEntityJsonViewModel<WeChatMessageAndUserViewModel> BindMessageViewModel(IList<WeChatMessageAndUserViewModel> messages, int total)
        {
            foreach (var item in messages)
            {
                item.MsgRenderContent = RequestMsgHelper.CreateMessageContentRenderString(item.MsgContent);
                item.MsgTypeString = RequestMsgHelper.CreateMsgTypeToName(item.MsgType.Value);
            }
            SNSEntityJsonViewModel<WeChatMessageAndUserViewModel> viewModel = new SNSEntityJsonViewModel<WeChatMessageAndUserViewModel>();
            viewModel.rows = messages;
            viewModel.total = total;
            return viewModel;
        }

        public ActionResult WeChatMessageList()
        {
            return View();
        }
    }
}
