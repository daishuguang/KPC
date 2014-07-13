using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text.RegularExpressions;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Open.Controllers
{
	public class PromoterController : Base.BaseController
    {
		#region "Action: Rank"
		[Authorize]
		public ActionResult Rank()
		{
			var client = CreateServiceClient<IRepositoryService>();

			try
			{
				return View(client.LoadPromoterRank());
			}
			finally
			{
				CloseServiceClient(client);
			}
		} 
		#endregion
	}
}