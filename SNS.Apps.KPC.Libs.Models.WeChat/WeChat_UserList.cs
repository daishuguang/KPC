using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Models.WeChat
{
    [Serializable]
    [DataContract]
    public class OpenIDList
    {
        [DataMember]
        [Newtonsoft.Json.JsonProperty("openid")]
        public List<string> OpenID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class WeChat_UserList
    {
        #region "Properties"

        [DataMember]
        [Newtonsoft.Json.JsonProperty("total")]
        public int Total { get; set; }

        [DataMember]
        [Newtonsoft.Json.JsonProperty("count")]
        public int Count { get; set; }

        [DataMember]
        [Newtonsoft.Json.JsonProperty("data")]
        public OpenIDList Data { get; set; }

        [DataMember]
        [Newtonsoft.Json.JsonProperty("next_openid")]
        public string Next_OpenID { get; set; }

        #endregion
    }
}
