using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SNS.Apps.KPC.Admin.Models;

namespace SNS.Apps.KPC.Admin.IServices
{
    public interface IWeChatMessageServices : IDisposable
    {
        IQueryable<WeChatMessage> GetMessages(int page, int rows, string sort, string order, out int total);
        IQueryable<ViewModels.WeChatMessageAndUserViewModel> GetMessagesContainsUser(int page, int rows, string sort, string order, Dictionary<string, object> search, out int total);
        WeChatMessage GetMessageById(long id);

    }
}