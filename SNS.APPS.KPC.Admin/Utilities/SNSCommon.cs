using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.APPS.BKED.Utilities
{
    public class SNSCommon
    {
        public static int ParseQueryStringToInt(string queryString, int defaultResult)
        {
            int result = defaultResult;
            if (int.TryParse(HttpContext.Current.Request.QueryString[queryString], out result))
            {
                return result;
            }
            return defaultResult;
        }
    }
}