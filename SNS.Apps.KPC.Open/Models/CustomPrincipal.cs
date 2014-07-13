using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Security.Principal;

namespace SNS.Apps.KPC.Open
{
	[Serializable]
	public class CustomPrincipal : IPrincipal
	{
		#region "Constructs"
		public CustomPrincipal(CustomPrincipalModel model)
		{
			this.Identity = new GenericIdentity(string.Format("{0} ({1}, {2})", model.NickName, model.ID, model.UserID.ToString()));
		} 
		#endregion

		public IIdentity Identity { get; private set; } 

		public bool IsInRole(string role)
		{
			return true;
		}
	}

	[Serializable]
	public class CustomPrincipalModel
	{
		#region "Properties"
		public long ID { get; set; }

		public Guid UserID { get; set; }

		public string NickName { get; set; }
		#endregion
	}
}