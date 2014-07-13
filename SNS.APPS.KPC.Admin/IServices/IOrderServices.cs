using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.IServices
{
    public interface IOrderServices : IDisposable
    {
        IQueryable<ViewModels.OrderViewModel> GetOrders(int page, int rows, string sort, string order, out int total);
        IQueryable<ViewModels.OrderViewModel> GetNewOrdersWithInintervalTime();
    }
}