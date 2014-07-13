using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.WeChat;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.WeChatHelper;

namespace SNS.Apps.TaskConsole
{
	class QRCodeGenerator
	{
		public static string Generate(int sceneNum)
		{
			//return WeChatHttpRequestor.CreateQRCode(QRCodeActionType.QR_LIMIT_SCENE, 14);
			return WeChatAPIExecutor.CreateQRCode(QRCodeActionType.QR_LIMIT_SCENE, sceneNum);
		}
	}
}
