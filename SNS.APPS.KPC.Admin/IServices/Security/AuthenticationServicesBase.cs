using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.IServices
{
    public abstract class AuthenticationServicesBase : IUserTokenServies
    {
        public ViewModels.CustomPrincipalSerializeModel UserPrincipalModel { get; private set; }
        string UsersConfigKey = "UsersConfigKey";
        string PasswordConfigKey = "PasswordConfigKey";

        public AuthenticationServicesBase()
        {
            UserPrincipalModel = new ViewModels.CustomPrincipalSerializeModel();
        }

        public virtual bool Authenticate(string username, string password)
        {
            var isValid = false;
            var authorizedUsers = ConfigurationManager.AppSettings[UsersConfigKey];
            var authorizedPassword = ConfigurationManager.AppSettings[PasswordConfigKey];
            if (username.Equals(authorizedUsers) && password.Equals(authorizedPassword))
            {
                UserPrincipalModel.UserId = Guid.NewGuid();
                UserPrincipalModel.FirstName = username;
                UserPrincipalModel.LastName = username;
                UserPrincipalModel.CreatedTime = System.DateTime.Now;
                UserPrincipalModel.Roles = new string[] { "Admin" };
                GenerateAuthorizedUserToken();
                isValid = true;
            }
            return isValid;
        }

        public virtual void GenerateAuthorizedUserToken()
        {
            return;
        }
    }
}