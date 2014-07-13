using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.DataStores;
using SNS.Apps.KPC.Libs.Models.SMS;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Open.Controllers
{
    public class APIController : Base.BaseController
    {
        #region "Fields"
        static string[] a_city = { "阿城市", "阿尔山市", "阿克苏市", "阿勒泰市", "阿图什市", "鞍山市", "安达市", "安国市", "安康市", "安陆市", "安宁市", "安庆市", "安丘市", "安顺市", "安阳市", "澳门" };
        static string[] b_city = { "巴中市", "霸州市", "白城市", "百色市", "白山市", "白银市", "保定市", "保山市", "宝鸡市", "包头市", "蚌埠市", "北安市", "北海市", "北京市", "北流市", "北宁市", "北票市", "本溪市", "毕节市", "滨州市", "博乐市", "泊头市", "亳州市" };
        static string[] c_city = { "沧州市", "岑溪市", "常德市", "常宁市", "常熟市", "长春市", "长葛市", "长乐市", "长沙市", "昌吉市", "昌邑市", "长治市", "常州市", "巢湖市", "朝阳市", "潮州市", "承德市", "成都市", "郴州市", "赤壁市", "赤峰市", "赤水市", "慈溪市", "重庆市", "崇州市", "从化市", "楚雄市", "滁州市" };
        static string[] d_city = { "达州市", "大安市", "大丰市", "大理市", "大连市", "大庆市", "大石桥市", "大同市", "大冶市", "丹东市", "丹江口市", "丹阳市", "当阳市", "儋州市", "德惠市", "德令哈市", "德兴市", "德阳市", "德州市", "登丰市", "灯塔市", "邓州市", "定州市", "钓鱼岛", "东方市", "东港市", "东莞市", "东胜市", "东台市", "东兴市", "东阳市", "东营市", "都江堰市", "都匀市", "敦化市", "敦煌市" };
        static string[] e_city = { "峨眉山市", "鄂尔多斯", "额尔古纳市", "鄂州市", "恩平市", "恩施市", "二连浩特市" };
        static string[] f_city = { "防城港市", "肥城市", "丰城市", "凤城市", "奉化市", "丰南市", "丰镇市", "汾阳市", "佛山市", "福安市", "福鼎市", "福清市", "福泉市", "福州市", "抚顺市", "阜康市", "阜新市", "阜阳市", "富锦市", "富阳市" };
        static string[] g_city = { "盖州市", "藁城市", "赣州市", "高安市", "高碑店市", "高密市", "高平市", "高要市", "高邮市", "高州市", "甘孜藏族自治州", "个旧市", "根河市", "格尔木市", "巩义市", "公主岭市", "古交市", "广安市", "广汉市", "广水市", "广元市", "广州市", "贵池市", "贵港市", "贵溪市", "贵阳市", "桂林市", "桂平市" };
        static string[] h_city = { "哈密市", "哈尔滨市", "海城市", "海口市", "海拉尔市", "海林市", "海伦市", "海门市", "海宁市", "海阳市", "韩城市", "汉川市", "汉中市", "邯郸市", "杭州市", "河池市", "河间市", "河津市", "河源市", "合川市", "合肥市", "合山市", "合作市", "和龙市", "菏泽市", "贺州市", "鹤壁市", "鹤岗市", "鹤山市", "黑河市", "衡水市", "衡阳市", "洪湖市", "洪江市", "和田市", "侯马市", "桦甸市", "呼和浩特市", "虎林市", "葫芦岛市", "湖州市", "华阴市", "华蓥市", "化州市", "淮安市", "淮北市", "淮南市", "淮阴市", "怀化市", "黄冈市", "黄骅市", "黄山市", "黄石市", "黄岩岛", "辉县市", "惠州市", "珲春市", "霍州市", "霍林郭勒市" };
        static string[] j_city = { "鸡西市", "冀州市", "集安市", "集宁市", "吉安市", "吉林市", "吉首市", "即墨市", "济宁市", "济源市", "济南市", "佳木斯市", "嘉兴市", "嘉峪关市", "简阳市", "建阳市", "建瓯市", "建德市", "姜堰市", "江都市", "江津市", "江门市", "江山市", "江阴市", "江油市", "蛟河市", "胶南市", "胶州市", "焦作市", "界首市", "介休市", "揭阳市", "金昌市", "晋城市", "晋中市", "晋江市", "景德镇市", "景洪市", "井冈山市", "靖江市", "荆门市", "荆州市", "金华市", "金坛市", "津市市", "锦州市", "酒泉市", "九江市", "九台市", "句容市" };
        static string[] k_city = { "凯里市", "开封市", "开平市", "开原市", "开远市", "克拉玛依市", "喀什市", "库尔勒市", "昆明市", "昆山市", "奎屯市" };
        static string[] l_city = { "莱芜市", "莱西市", "莱阳市", "莱州市", "廊坊市", "阆中市", "兰溪市", "兰州市", "老河口市", "乐昌市", "耒阳市", "雷州市", "乐陵市", "冷水江市", "乐平市", "乐山市", "拉萨市", "廉江市", "涟源市", "连云港市", "连州市", "聊城市", "辽阳市", "辽源市", "利川市", "醴陵市", "灵宝市", "凌海市", "灵武市", "凌源市", "临海市", "临河市", "临江市", "临清市", "临夏市", "临湘市", "临沂市", "临安市", "临川市", "临汾市", "林州市", "离石市", "丽水市", "六安市", "六盘水市", "浏阳市", "柳州市", "溧阳市", "龙海市", "龙井市", "龙口市", "龙泉市", "龙岩市", "娄底市", "潞城市", "陆丰市", "罗定市", "漯河市", "洛阳市", "鹿泉市", "潞西市", "泸州市" };
        static string[] m_city = { "马鞍山市", "麻城市", "满洲里市", "茂名市", "梅河口市", "梅州市", "孟州市", "绵阳市", "汨罗市", "明光市", "米泉市", "密山市", "牡丹江市", "穆棱市" };
        static string[] n_city = { "南安市", "南昌市", "南充市", "南川市", "南宫市", "南京市", "南康市", "南宁市", "南平市", "南通市", "南雄市", "南阳市", "讷河市", "内江市", "宁安市", "宁波市", "宁德市", "宁国市", "那曲地区" };
        static string[] p_city = { "盘锦市", "磐石市", "攀枝花市", "蓬莱市", "彭州市", "平顶山市", "平度市", "平湖市", "平凉市", "萍乡市", "凭祥市", "邳州市", "普兰店市", "普宁市", "莆田市", "濮阳市" };
        static string[] q_city = { "迁安市", "潜江市", "启东市", "青岛市", "青铜峡市", "庆阳市", "清远市", "清镇市", "青州市", "秦皇岛市", "沁阳市", "钦州市", "邛崃市", "琼海市", "琼山市", "齐齐哈尔市", "七台河市", "栖霞市", "泉州市", "曲阜市", "曲靖市", "衢州市" };
        static string[] r_city = { "仁怀市", "仁爱礁", "任丘市", "日照市", "日喀则市", "荣城市", "如皋市", "乳山市", "汝州市", "瑞安市", "瑞昌市", "瑞金市", "瑞丽市" };
        static string[] s_city = { "三河市", "三门峡市", "三明市", "三亚市", "三沙市", "沙河市", "汕头市", "汕尾市", "上海市", "上饶市", "上虞市", "尚志市", "商丘市", "商州市", "韶关市", "韶山市", "邵武市", "邵阳市", "绍兴市", "沈阳市", "深圳市", "深州市", "什邡市", "嵊州市", "石河子市", "石家庄市", "石狮市", "石首市", "石嘴山市", "寿光市", "十堰市", "双城市", "双辽市", "双鸭山市", "舒兰市", "朔州市", "思茅市", "四会市", "四平市", "松原市", "松滋市", "宿迁市", "宿州市", "苏州市", "绥芬河市", "绥化市", "遂宁市", "随州市" };
        static string[] t_city = { "塔城市", "泰安市", "泰兴市", "太仓市", "太原市", "泰州市", "台山市", "台北市", "台州市", "唐山市", "洮南市", "滕州市", "天长市", "天津市", "天门市", "天水市", "铁法市", "铁力市", "铁岭市", "桐城市", "桐乡市", "铜川市", "铜仁市", "铜陵市", "同江市", "通化市", "通辽市", "通什市", "通州市", "图们市", "吐鲁番市" };
        //static string[] u_city = {  };
        static string[] w_city = { "瓦房店市", "畹町市", "万宁市", "万源市", "潍坊市", "威海市", "卫辉市", "渭南市", "文昌市", "文登市", "文水市", "温岭市", "温州市", "武安市", "五常市", "吴川市", "五大连池市", "武冈市", "舞钢市", "乌海市", "乌鲁木齐市", "乌兰浩特市", "乌苏市", "武汉市", "芜湖市", "武进市", "武威市", "无锡市", "武穴市", "武夷山市", "吴县市", "吴江市", "吴忠市", "梧州市" };
        static string[] x_city = { "西安市", "西昌市", "西峰市", "西宁市", "锡林浩特市", "锡山市", "厦门市", "咸宁市", "仙桃市", "咸阳市", "项城市", "襄樊市", "香港", "湘潭市", "湘乡市", "孝感市", "萧山市", "孝义市", "辛集市", "新乐市", "新密市", "新民市", "新泰市", "新乡市", "新沂市", "新余市", "新郑市", "信阳市", "信宜市", "忻州市", "兴城市", "兴化市", "兴宁市", "兴平市", "兴义市", "邢台市", "荥阳市", "许昌市", "徐州市", "宣威市", "宣州市" };
        static string[] y_city = { "雅安市", "牙克石市", "延安市", "延吉市", "盐城市", "偃师市", "烟台市", "兖州市", "阳春市", "阳江市", "阳泉市", "扬中市", "扬州市", "伊春市", "宜宾市", "宜昌市", "宜城市", "宜春市", "宜都市", "宜州市", "宜兴市", "义马市", "义乌市", "仪征市", "伊宁市", "益阳市", "银川市", "应城市", "英德市", "营口市", "鹰潭市", "永安市", "永城市", "永川市", "永济市", "永康市", "永州市", "余杭市", "余姚市", "榆林市", "榆树市", "沅江市", "原平市", "禹城市", "乐清市", "岳阳市", "运城市", "云浮市", "玉林市", "玉门市", "玉溪市" };
        static string[] z_city = { "枣阳市", "枣庄市", "增城市", "扎兰屯市", "湛江市", "张家港市", "张家界市", "张家口市", "张掖市", "章丘市", "樟树市", "漳州市", "漳平市", "肇东市", "肇庆市", "昭通市", "招远市", "镇江市", "郑州市", "枝江市", "中山市", "周口市", "舟山市", "庄河市", "诸城市", "珠海市", "诸暨市", "驻马店市", "涿州市", "株洲市", "淄博市", "自贡市", "资兴市", "资阳市", "邹城市", "遵化市", "遵义市" };

        static Hashtable _htCities = null;
        #endregion

        #region "Constructs"
        static APIController()
        {
            if (_htCities == null)
            {
                _htCities = new Hashtable();

                _htCities.Add("a_city", a_city);
                _htCities.Add("b_city", b_city);
                _htCities.Add("c_city", c_city);
                _htCities.Add("d_city", d_city);
                _htCities.Add("e_city", e_city);
                _htCities.Add("f_city", f_city);
                _htCities.Add("g_city", g_city);
                _htCities.Add("h_city", h_city);
                _htCities.Add("j_city", j_city);
                _htCities.Add("k_city", k_city);
                _htCities.Add("l_city", l_city);
                _htCities.Add("m_city", m_city);
                _htCities.Add("n_city", n_city);
                _htCities.Add("p_city", p_city);
                _htCities.Add("q_city", q_city);
                _htCities.Add("r_city", r_city);
                _htCities.Add("s_city", s_city);
                _htCities.Add("t_city", t_city);
                //_htCities.Add("u_city", u_city);
                _htCities.Add("w_city", w_city);
                _htCities.Add("x_city", x_city);
                _htCities.Add("y_city", y_city);
                _htCities.Add("z_city", z_city);
            }
        }
        #endregion

        #region "API: UserAround"
        [Authorize]
        [Filters.UserAuthNoRegisterFilter]
        public JsonResult Around(float ld_lng, float ld_lat, float rt_lng, float rt_lat)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                return Json(new RequestResult<UserTrackResult>
                {
                    Status = RequestStatus.OK,
                    Data = client.LoadUserAround(new UserTrackRequestMap
                    {
                        Filter = new UserTrackRequest.UserTrackRequestFilter { UserID = this.CurrentUser.UserGUID, Range = new Range { BL = new Point { Longitude = ld_lng, Latitude = ld_lat }, TR = new Point { Longitude = rt_lng, Latitude = rt_lat } } }
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DBLogger.Instance.ErrorFormat("Action: string LoadAround(string userGUID, float lng_BL, float lat_BL, float lng_TR, float lat_TR, string signcode)\r\n" +
                    "Parameters: userGUID: {0}, ld_lng: {1}, ld_lat: {2}, rt_lng: {3}, rt_lat: {4},\r\nException: {5}",
                    this.CurrentUser.UserGUID,
                    ld_lng,
                    ld_lat,
                    rt_lng,
                    rt_lat,
                    ex.ToString());

                return Json(new RequestResult { Status = RequestStatus.Error, Message = ex.Message });
            }
            finally
            {
                CloseServiceClient(client);
            }
        }

        [Authorize]
        [Filters.UserAuthNoRegisterFilter]
        public JsonResult AroundListMore(int pagenum, string role = "all")
        {
            var client = CreateServiceClient<IRepositoryService>();
            var userrole = default(Nullable<UserRole>);
            switch (role)
            {
                case "passenger":
                    userrole = UserRole.Passenger;
                    break;
                case "driver":
                    userrole = UserRole.Driver;
                    break;
            }

            try
            {
                var AroundListModel = client.LoadUserAround(new UserTrackRequestList
                {
                    Filter = new UserTrackRequest.UserTrackRequestFilter
                    {
                        UserID = this.CurrentUser.UserGUID,
                        UserRole = userrole
                    },
                    Page = pagenum
                });

                var flag = true;

                if (AroundListModel.Count() == 0)
                {
                    flag = false;
                }

                return Json(new { flag = flag, dataList = AroundListModel }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DBLogger.Instance.ErrorFormat(
                    "Controller: {0}, Action: {1}\r\n Parameters: {2}\r\nException: {3}",
                    "UserController",
                    "AroundList",
                    string.Empty,
                    ex.ToString()
                    );

                return Json(new RequestResult { Status = RequestStatus.Error, Message = ex.Message });
            }
            finally
            {
                CloseServiceClient(client);
            }
        }

        [Authorize]
        [Filters.UserAuthNoRegisterFilter]
        public JsonResult SetUserTrack(float lng, float lat)
        {
            var clientR = CreateServiceClient<IRepositoryService>();

            try
            {
                clientR.SetUserTrack(this.CurrentUser.ID, new Point { Longitude = lng, Latitude = lat });

                return Json(new RequestResult { Status = RequestStatus.OK }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new RequestResult { Status = RequestStatus.Error, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                CloseServiceClient(clientR);
            }
        }

        #endregion

        #region "API: UserRoute"
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SearchResult(string from_location, string from_province, string from_city, string from_district, string to_location, string to_province, string to_city, string to_district, string userrole, int pagenum, string ptype, string pcon, string pcon2, string pdate = null, double from_lat = 0.0, double from_lng = 0.0, double to_lat = 0.0, double to_lng = 0.0)
        {
            var client = CreateServiceClient<IRouteMatrixService>();
            var role = default(Nullable<UserRole>);

            switch (userrole)
            {
                case "passenger":
                    role = UserRole.Passenger;
                    break;
                case "driver":
                    role = UserRole.Driver;
                    break;
                case "all":
                    role = null;
                    break;
            }

            try
            {
                var requestinfo = new RouteSearchRequest
                {
                    Filter = new RouteSearchFilter
                    {
                        LocationFilter = new RouteSearch_LocationFilter
                        {
                            #region "Location: From"
                            From_Location = from_location,
                            From_Province = from_province,
                            From_City = from_city,
                            From_District = from_district,

                            From_Point = new Point
                            {
                                Longitude = from_lng,
                                Latitude = from_lat
                            },
                            #endregion

                            #region "Location: To"
                            To_Location = to_location,
                            To_Province = to_province,
                            To_City = to_city,
                            To_District = to_district,

                            To_Point = new Point
                            {
                                Latitude = to_lat,
                                Longitude = to_lng
                            }
                            #endregion
                        },
                        DateFilter = new RouteSearch_DateFilter
                        {
                            Date = (!string.IsNullOrEmpty(pdate)) ? (Convert.ToDateTime(pdate)) : ((DateTime?)null),
                            Range = ((!string.IsNullOrEmpty(pcon)) ? (Convert.ToInt32(pcon)) : (0)) | ((!string.IsNullOrEmpty(pcon2)) ? (Convert.ToInt32(pcon2)) : (0))
                        },
                        UserRole = role
                    },
                    Page = pagenum
                };

                var flag = true;
                var routesearchResult = client.Search(requestinfo);

                routesearchResult = (routesearchResult != null ? routesearchResult : new RouteSearchResult[] { });

                if (routesearchResult.Count() == 0)
                {
                    flag = false;
                }

                return Json(new { flag = flag, dataList = routesearchResult });
            }
            finally
            {
                CloseServiceClient(client);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteRoute(Guid userID, Guid routeID)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                #region "验证参数"
                if (!userID.Equals(this.CurrentUser.UserGUID))
                {
                    throw new UnauthorizedAccessException("您无权执行此操作！");
                }

                var ur = client.GetUserRouteByRouteID(routeID);

                if (ur == null || ur.Route == null)
                {
                    throw new Exception(string.Format("无效的参数 routeID：{0}", routeID));
                }
                else if (!ur.User.UserGUID.Equals(userID))
                {
                    throw new UnauthorizedAccessException("您无权执行此操作！");
                }
                #endregion

                client.DeleteRoute(ur.Route.ID);

                return Json(new RequestResult { Status = RequestStatus.OK });
            }
            finally
            {
                CloseServiceClient(client);
            }
        }

        [AllowAnonymous]
        public ActionResult PlazaMore(string city, int pagenum)
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                if (string.IsNullOrEmpty(city))
                {
                    var userID = (this.CurrentUser != null) ? ((int?)this.CurrentUser.ID) : ((int?)null);
                    var userTrack = (userID != null && userID.HasValue) ? (client.GetUserTrack(userID.Value)) : (null);

                    #region "获取当前所在城市"
                    if (userTrack != null)
                    {
                        if (userTrack.Route != null)
                        {
                            city = userTrack.Route.From_City;
                        }
                        else if (userTrack.Position != null && userTrack.Position.HasValue)
                        {
                            var loc = Location.Reverse(userTrack.Position.Value);

                            if (loc != null)
                            {
                                city = loc.City;
                            }
                        }
                    }
                    #endregion
                }

                var flag = true;
                var results = client.LoadUserRouteNewest(new RouteSearchRequest { Filter = new RouteSearchFilter { LocationFilter = new RouteSearch_LocationFilter { From_City = city } }, Page = pagenum, Count = 10 });

                if (results == null || results.Count() == 0)
                {
                    flag = false;
                }

                return Json(new { flag = flag, dataList = results }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                CloseServiceClient(client);
            }
        }
        #endregion

        #region "API: UserOrder"
        [HttpGet]
        [Authorize]
        public JsonResult LoadUserOrders()
        {
            var client = CreateServiceClient<IRepositoryService>();

            try
            {
                var orderType = (!string.IsNullOrEmpty(Request.QueryString["orderType"])) ? (Convert.ToInt32(Request.QueryString["orderType"])) : ((int)OrderType.All);
                var pagenum = (!string.IsNullOrEmpty(Request.QueryString["pagenum"])) ? (Convert.ToInt32(Request.QueryString["pagenum"])) : (0);

                var uos = client.GetUserOrder(new UserOrderRequest
                {
                    Filter = new UserOrderRequest.UserOrderRequestFilter
                    {
                        UserID = this.CurrentUser.UserGUID,
                        OrderType = (OrderType)Enum.Parse(typeof(OrderType), orderType.ToString())
                    },
                    Page = pagenum
                });

                var flag = true;

                if (uos == null || uos.Count() == 0)
                {
                    flag = false;
                }

                return Json(new { flag = flag, dataList = uos }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                CloseServiceClient(client);
            }
        }
        #endregion

        #region "API: VerifyPhoneNumber"
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VerifyPhoneNumber(string phonenum, int channel)
        {
            var refUrl = string.Format("http://{0}{1}", Request.UrlReferrer.Host, ((Request.UrlReferrer.Port == 80) ? (string.Empty) : (":" + Request.UrlReferrer.Port)));

            if (string.Compare(refUrl, ConfigStore.CommonSettings.App_Site_Url, true) == 0)
            {
                var clientS = CreateServiceClient<ISMSService>();
                var clientR = CreateServiceClient<IRepositoryService>();

                try
                {
                    var code = clientR.SetVerifyCode(phonenum, channel);

                    if (!string.IsNullOrEmpty(code))
                    {
                        var smsResult = clientS.SendSMS_VerifyCode(new SMSMessageSendRequest
                        {
                            Content = string.Format(GenerateSMSContent(channel), code),
                            Mobiles = new String[] { phonenum },
                            Channel = channel
                        });

                        if (smsResult.Success)
                        {
                            return Json(new { flag = true });
                        }
                    }

                    return Json(new { flag = false });
                }
                finally
                {
                    CloseServiceClient(clientS);
                    CloseServiceClient(clientR);
                }
            }
            else
            {
                return Json(new { flag = false });
            }
        }

        string GenerateSMSContent(int channel)
        {
            var content = string.Empty;

            switch (channel)
            {
                case 0:
                    content = "欢迎使用快拼车，您的验证码是：{0}";
                    break;
                case 1:
                    content = "欢迎使用快拼车，您的验证码是：{0}";
                    break;
            }

            return content;
        }
        #endregion

        #region "API: LoadCity"
        [AllowAnonymous]
        public ActionResult LoadCity(string l)
        {
            var targetCity = (string[])_htCities[l.ToLower() + "_city"];

            return Json(new { cityList = targetCity }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "API: GetEnumDescription"
        [AllowAnonymous]
        public string GetEnumDescription(string enumType, int enumValue)
        {
            var t = Type.GetType(enumType);

            if (t == null)
            {
                return string.Empty;
            }

            var enumItem = Enum.Parse(t, enumValue.ToString());

            if (enumItem == null)
            {
                return string.Empty;
            }

            return ((Enum)enumItem).GetDescription();
        }
        #endregion
    }
}
