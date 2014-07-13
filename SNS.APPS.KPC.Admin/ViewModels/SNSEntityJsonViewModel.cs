using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.ViewModels
{
    public class SNSEntityJsonViewModel<TEntity> where TEntity : class
    {
        public int total { get; set; }
        public IList<TEntity> rows { get; set; }
    }
}