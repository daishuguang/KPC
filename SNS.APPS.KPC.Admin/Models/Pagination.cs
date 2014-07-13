using SNS.Apps.KPC.Admin.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNS.Apps.KPC.Admin.Models
{
    public class Pagination
    {
        public Pagination()
        {
            this.Page = 1;
            this.Rows = 10;
            this.Search = "";
            this.Order = "desc";
            this.Sort = "CreateDate";
        }
        public int Page { get; set; }
        public int Rows { get; set; }
        public string Search { get; set; }
        public string Order { get; set; }
        public string Sort { get; set; }

    }

    public class PaginationDataBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            HttpRequestBase request = controllerContext.HttpContext.Request;
            int page = CommonHelper.ParseInt(CommonHelper.GetRequestParam(request, "page"), 1);
            int rows = CommonHelper.ParseInt(CommonHelper.GetRequestParam(request, "rows"), 10);
            string order = CommonHelper.GetRequestParam(request, "order");
            string sort = CommonHelper.GetRequestParam(request, "sort");
            string search = CommonHelper.GetRequestParam(request, "search");

            return new Pagination
            {
                Page = page,
                Rows = rows,
                Order = order,
                Sort = sort,
                Search = search
            };

        }
    }
}