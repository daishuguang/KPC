using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.Services
{
    public abstract class BaseServices : IDisposable
    {
        private bool disposed = false;
        public SNSDataContext DataContext { get; set; }
        public BaseServices()
        {
            DataContext = new SNSDataContext();
        }


        #region IDisposable 成员
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                this.DataContext.Dispose();
                //this.Dispose();
                disposed = true;
            }
            disposed = true;
        }

    }


}