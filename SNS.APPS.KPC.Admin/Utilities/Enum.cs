using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNS.Apps.KPC.Admin.Utilities
{
    public class EnumType
    {
        public enum RequestMsgType
        {
            文本 = 0, //文本
            图片, //图片
            视频, //视频
            语音, //语音
            地理位置, //地理位置
            连接信息, //连接信息
            事件推送, //事件推送
        }

        public enum StatusCode
        {
            Success = 200,
            Error = 400,
            UnknowException = 500
        }

        public enum RouteType
        {
            上下班 = 0,
            长途 = 1
        }

        public enum UserRole
        {
            乘客 = 0,
            司机 = 1
        }

        public enum OrderType
        {
            等待支付 = 0,
            已付款 = 1,
            已完成 = 100,
            已过期 = -1,
            已取消 = -2,
            已申请退款 = -3,
            已退款 = -4
        }
    }
}