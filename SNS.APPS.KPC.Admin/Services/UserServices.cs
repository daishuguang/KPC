using SNS.Apps.KPC.Admin.IServices;
using SNS.Apps.KPC.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using KPCLibs = SNS.Apps.KPC.Libs.Models;

namespace SNS.Apps.KPC.Admin.Services
{
    public class UserServices : BaseServices, IUserServices
    {
        //SNSDataContext db;
        KPCLibs.DataStores.KPCDataModels kpcDb;
        public UserServices()
            : base()
        {
            //db = new SNSDataContext();
            kpcDb = new KPCLibs.DataStores.KPCDataModels();
        }

        public Users GetUserById(int id)
        {
            return DataContext.Users.Where(u => u.ID.Equals(id)).FirstOrDefault();
        }

        public Users GetUser(string nickName, string mobile)
        {
            Users user = new Users();
            var query = from u in DataContext.Users select u;
            if (nickName!= null)
                query = DataContext.Users.Where(u => u.NickName.Equals(nickName));
            if (mobile != null)
                query = DataContext.Users.Where(u => u.Mobile.Equals(mobile));

            if (query.Count() > 0)
                user = query.First();
            return user;
        }

        public KPCLibs.DataStores.tbl_User GetKpcUserById(int id)
        {
            return kpcDb.tbl_User.Where(u => u.ID.Equals(id)).FirstOrDefault();
        }
    }
}