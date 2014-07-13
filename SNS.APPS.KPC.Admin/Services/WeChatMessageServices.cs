using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using SNS.Apps.KPC.Admin.IServices;
using System.Text;
using System.Linq.Expressions;
using SNS.Apps.KPC.Admin.Services;


namespace SNS.Apps.KPC.Admin.Services
{
    public class WeChatMessageServices : BaseServices, IWeChatMessageServices
    {
        //SNSDataContext db;
        public WeChatMessageServices()
            : base()
        {
            //db = new SNSDataContext();
        }

        public IQueryable<WeChatMessage> GetMessages(int page, int rows, string sort, string order, out int total)
        {
            //total = GetTotals();
            string orderStr = string.Format("{0} {1}", sort, order);
            var query = (from q in DataContext.WeChatMessages
                         join u in DataContext.Users on q.From_OpenID equals u.OpenID
                         select q);
            total = query.Count();
            query = query.OrderBy(orderStr)
                        .Skip((page - 1) * rows)
                        .Take(rows);

            return query;
        }

        public IQueryable<ViewModels.WeChatMessageAndUserViewModel> GetMessagesContainsUser(int page, int rows, string sort, string order, Dictionary<string, object> search, out int total)
        {
            string orderStr = string.Format("{0} {1}", sort, order);


            var query = (from q in DataContext.WeChatMessages
                         join u in DataContext.Users on q.From_OpenID equals u.OpenID
                         select new ViewModels.WeChatMessageAndUserViewModel
                         {
                             UserID = u.ID,
                             PortraitsThumbUrl = u.PortraitsThumbUrl,
                             PortraitsUrl = u.PortraitsUrl,
                             OpenID = u.OpenID,
                             FakeID = u.FakeID,
                             NickName = u.NickName,
                             Gender = u.Gender,
                             City = u.City,
                             Country = u.Country,
                             To_OpenID = q.To_OpenID,
                             MsgContent = q.MsgContent,
                             MsgType = q.MsgType,
                             MsgID = q.MsgID,
                             From_OpenID = q.From_OpenID,
                             CreateDate = q.CreateDate,

                         });

            Expression<Func<ViewModels.WeChatMessageAndUserViewModel, bool>> whereExpression = m => 1 == 1;
            foreach (var item in search)
            {
                if (item.Key.Equals("MsgType"))
                {
                    var msgType = int.Parse(item.Value.ToString());
                    whereExpression = m => m.MsgType == msgType;
                }
                if (item.Key.Equals("MsgContent"))
                {
                    var msgContent = item.Value.ToString();
                    whereExpression = m => m.MsgContent.Contains(msgContent);
                }
                if (item.Key.Equals("NickName"))
                {
                    var nickName = item.Value.ToString();
                    whereExpression = m => m.NickName == nickName;
                }
                query = query.Where(whereExpression);
            }
            total = query.Count();
            query = query.OrderBy(orderStr)
                         .Skip((page - 1) * rows)
                         .Take(rows);
            //total = GetTotals(whereExpression);

            return query;
        }

        public WeChatMessage GetMessageById(long id)
        {
            var item = DataContext.WeChatMessages.Where(m => m.ID == id).FirstOrDefault();
            if (item == null)
                return new WeChatMessage { ID = 0 };
            return item;
        }

        //public int GetTotals()
        //{
        //    return (from q in db.WeChatMessages
        //            join u in db.Users on q.From_OpenID equals u.OpenID
        //            select q).Count();
        //}

        //public int GetTotals(Expression<Func<ViewModels.WeChatMessageAndUserViewModel, bool>> expression)
        //{
        //    int count = 0;
        //    var query = (from q in db.WeChatMessages
        //                 join u in db.Users on q.From_OpenID equals u.OpenID
        //                 select new ViewModels.WeChatMessageAndUserViewModel
        //                 {
        //                     NickName = u.NickName,
        //                     City = u.City,
        //                     Country = u.Country,
        //                     MsgContent = q.MsgContent,
        //                     MsgType = q.MsgType.Value,
        //                     MsgID = q.MsgID,
        //                     From_OpenID = q.From_OpenID,
        //                     CreateDate = q.CreateDate,
        //                 });
        //    if (expression != null)
        //        query = query.Where(expression);
        //    count = query.Count();

        //    return count;
        //}

    }
}