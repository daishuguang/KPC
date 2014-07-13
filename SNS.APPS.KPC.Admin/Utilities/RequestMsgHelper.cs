using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SNS.Apps.KPC.Admin.Utilities
{
    public class RequestMsgHelper
    {
        public static string CreateMessageContentRenderString(string xml)
        {
            StringBuilder sb = new StringBuilder();
            Senparc.Weixin.MP.Entities.IRequestMessageBase requestMessage = RequestMessageFactory.GetRequestEntity(xml);
            switch (requestMessage.MsgType)
            {
                case Senparc.Weixin.MP.RequestMsgType.Text:
                    sb.AppendFormat("文字内容： {0}", (requestMessage as RequestMessageText).Content);
                    break;
                case Senparc.Weixin.MP.RequestMsgType.Location:
                    RequestMessageLocation requestLocation = requestMessage as RequestMessageLocation;
                    sb.AppendFormat("地理位置信息： {0}, X:{1}, Y{2}", requestLocation.Label, requestLocation.Location_X, requestLocation.Location_Y);
                    break;
                case Senparc.Weixin.MP.RequestMsgType.Image:
                    RequestMessageImage requestImage = requestMessage as RequestMessageImage;
                    sb.AppendFormat("图片地址URL： {0}", requestImage.PicUrl);
                    break;
                case Senparc.Weixin.MP.RequestMsgType.Video:
                    RequestMessageVideo requestVideo = requestMessage as RequestMessageVideo;
                    sb.AppendFormat("媒体文件ID： {0}", requestVideo.MediaId);
                    break;
                default:
                    sb.AppendFormat("无法识别消息类型");
                    break;

            }
            return sb.ToString();
        }

        public static string CreateMsgTypeToName(int msgType)
        {
            return Enum.GetName(typeof(SNS.Apps.KPC.Admin.Utilities.EnumType.RequestMsgType), msgType);
        }

        public static int CreateMsgTypeFromName(string msgType)
        {
            int type = 0;
            try
            {
                type = (int)Enum.Parse(typeof(SNS.Apps.KPC.Admin.Utilities.EnumType.RequestMsgType), msgType);
            }
            catch
            {
                type = -1;
            }
            return type;
        }
    }
}