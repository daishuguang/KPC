using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using SNS.Apps.KPC.Admin.Utilities;
using SNS.Apps.KPC.Admin.IServices;
using System.Linq.Expressions;


namespace SNS.Apps.KPC.Admin.Services
{
    public class RouteServices : BaseServices, IRouteServices
    {
        //SNSDataContext db;
        public RouteServices()
            : base()
        {
            //db = new SNSDataContext();
        }

        public IQueryable<ViewModels.RouteViewModel> GetRoutes(int page, int rows, string sort, string order, out int total, KeyValuePair<string, string> searchPair)
        {
            total = 0;
            string orderStr = string.Format("{0} {1}", sort, order);
            var query = DataContext.Routes
                           .Join(DataContext.User_Route,
                           r => r.ID,
                           ur => ur.RouteID,
                           (r, ur) => new { Routes = r, User_Routes = ur }
                          ).Join(
                          DataContext.Users,
                          u => u.User_Routes.UserID,
                          m => m.ID,
                          (u, m) => new ViewModels.RouteViewModel
                          {
                              PublisherID = m.ID,
                              PublisherNickName = m.NickName,
                              PublisherMobile = m.Mobile,
                              PublisherRoleType = u.User_Routes.UserRole,
                              ID = u.Routes.ID,
                              From_City = u.Routes.From_City,
                              From_District = u.Routes.From_District,
                              From_Location = u.Routes.From_Location,
                              From_Province = u.Routes.From_Province,
                              To_City = u.Routes.To_City,
                              To_District = u.Routes.To_District,
                              To_Location = u.Routes.To_Location,
                              To_Province = u.Routes.To_Province,
                              RouteType = u.Routes.RouteType,
                              StartDate = u.Routes.StartDate,
                              Charge = u.Routes.Charge,
                              CreateDate = u.Routes.CreateDate,
                              UpdateDate = u.Routes.UpdateDate,
                          });
            Expression<Func<ViewModels.RouteViewModel, bool>> whereExpression = r => 1 == 1;
            if (!string.IsNullOrEmpty(searchPair.Key))
            {
                switch (searchPair.Key)
                {
                    case "uid":
                        var uid = int.Parse(searchPair.Value);
                        whereExpression = r => r.PublisherID == uid;
                        break;
                    default:
                        break;
                }
            }
            query = query.Where(whereExpression);
            total = query.Count();
            query = query.OrderBy(orderStr)
                .Skip((page - 1) * rows)
                .Take(rows);

            return query;


        }

        public IQueryable<ViewModels.RouteViewModel> GetRoutesByUserID(int uid, int page, int rows, string sort, string order, out int total)
        {
            total = 0;
            KeyValuePair<string, string> searchPair = new KeyValuePair<string, string>("uid", uid.ToString());
            return this.GetRoutes(page, rows, sort, order, out total, searchPair);
        }



    }
}