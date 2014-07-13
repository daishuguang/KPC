using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.Utilities
{
    public class CommonHelper
    {

        public static int ParseInt(string value,int defValue)
        {
            int result = defValue;
            int.TryParse(value, out result);
            return result;
        }

        public static decimal ParseDecimal(string value, decimal defValue)
        {
            decimal result = defValue;
            decimal.TryParse(value, out result);
            return result;
        }

        public static string GetRequestParam(HttpRequestBase request,string paramName)
        {
            string value = string.Empty;
            value = request.QueryString.Get(paramName);
            if (string.IsNullOrEmpty(value))
            {
                value = request.Form.Get(paramName);
            }
            return value;
        }
    }
}