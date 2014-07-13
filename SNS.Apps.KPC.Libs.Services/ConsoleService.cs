using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Models.Repositories;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Services
{
	public class ConsoleService : IConsoleService
	{
		public void CleanMemCache()
		{
			const string CNSTR_MemCacheKey = "MemCacheKey";

			try
			{
				MemCacheWrapper.Remove(CNSTR_MemCacheKey, true);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
		}
	}
}
