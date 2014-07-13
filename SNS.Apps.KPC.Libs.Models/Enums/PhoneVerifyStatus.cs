using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SNS.Apps.KPC.Libs.Models
{
    public enum PhoneVerifyStatus
    {
        [Description("未验证")]
        UnVerified = 0,

        [Description("已验证")]
        Verified
    }
}
