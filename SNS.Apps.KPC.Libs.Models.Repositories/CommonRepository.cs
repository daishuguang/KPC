using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public sealed class CommonRepository : Base.RepositoryBase<CommonRepository>
	{
		#region "Constructs"
		private CommonRepository() { }
		#endregion

		#region "Methods: PhoneVerify"
		public bool VerifyMobile(string mobile, string code)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var result = ctx.sp_Check_PhoneVerifyCode(mobile, code).FirstOrDefault();

				return (result != null && result.HasValue && result.Value == 0);
			}
		}

		public string SetVerifyCode(string mobile, int channel)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var code = (new Random()).Next(1000, 9999).ToString();

				ctx.sp_Set_PhoneVerifyCode(mobile.ToString(), code, channel);

				return code;
			}
		}
		#endregion
	}
}
