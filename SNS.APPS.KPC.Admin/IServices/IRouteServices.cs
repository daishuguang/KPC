using SNS.Apps.KPC.Admin.Models;
using SNS.Apps.KPC.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.IServices
{
    public interface IRouteServices : IDisposable
    {
        IQueryable<RouteViewModel> GetRoutes(int page, int rows, string sort, string order, out int total, KeyValuePair<string, string> searchPair);
        IQueryable<RouteViewModel> GetRoutesByUserID(int uid, int page, int rows, string sort, string order, out int total);
    }
}