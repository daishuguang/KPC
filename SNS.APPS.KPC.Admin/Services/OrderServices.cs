using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.Models;
using SNS.Apps.KPC.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace SNS.Apps.KPC.Admin.Services
{
    public class OrderServices : BaseServices, IOrderServices
    {
        //SNSDataContext db;
        public OrderServices()
            : base()
        {
            //db = new SNSDataContext();
        }

        public IQueryable<ViewModels.OrderViewModel> GetOrders(int page, int rows, string sort, string order, out int total)
        {
            string orderStr = string.Format("{0} {1}", sort, order);
            var query = (from q in DataContext.User_Order
                         join u in DataContext.Users on q.RequestorID equals u.ID
                         join u2 in DataContext.Users on q.SupplierID equals u2.ID
                         join r in DataContext.Routes on q.RouteID equals r.ID

                         select new OrderViewModel
                         {
                             ID = q.ID,
                             Folio = q.Folio,
                             RequestorID = q.RequestorID,
                             SupplierID = q.SupplierID,
                             SupplierRole = q.SupplierRole,
                             RouteID = q.RouteID,
                             StartDate = q.StartDate,
                             CreateDate = q.CreateDate,
                             UpdateDate = q.UpdateDate,
                             RequestorName = u.NickName,
                             RequestorMobile = u.Mobile,
                             SupplierName = u2.NickName,
                             SupplierMobile = u2.Mobile,
                             IsCancelled_Requestor = q.IsCancelled_Requestor,
                             IsConfirmed_Requestor = q.IsConfirmed_Requestor,
                             IsCancelled_Supplier = q.IsCancelled_Supplier,
                             IsConfirmed_Supplier = q.IsConfirmed_Supplier,
                             FromLocation = r.From_Location,
                             ToLocation = r.To_Location,
                             Status = q.Status,
                         });
            total = query.Count();
            query = query.OrderBy(orderStr)
                .Skip((page - 1) * rows)
                .Take(rows);

            return query;
        }

        public IQueryable<OrderViewModel> GetNewOrdersWithInintervalTime()
        {
            int interval = int.Parse(ConfigurationManager.AppSettings["IntervalCheckTime"]);
            DateTime systemTime = System.DateTime.Now.AddMilliseconds(-interval);
            //DateTime systemTime = DateTime.Parse("2014-02-27 17:55:00");
            var query = DataContext.User_Order
                .Where(o => o.CreateDate > systemTime 
                    || o.UpdateDate>systemTime)
                .Select(r => new OrderViewModel
                {
                    ID = r.ID,
                    Folio = r.Folio
                });
            return query;
        }
    }
}