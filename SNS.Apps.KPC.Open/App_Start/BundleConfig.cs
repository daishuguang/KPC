using System.Web;
using System.Web.Optimization;

namespace SNS.Apps.KPC.Open
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymobile").Include(
                "~/Scripts/jquery.mobile*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            /* CSS START */
            bundles.Add(new StyleBundle("~/Content/main").Include(
                        "~/Content/view_source/main.css"));

            bundles.Add(new StyleBundle("~/Content/route/create").Include(
                        "~/Content/widget/mobiscroll.core.css",
                        "~/Content/view_source/create.css"));

            bundles.Add(new StyleBundle("~/Content/route/search").Include(
                        "~/Content/widget/mobiscroll.core.css",
                        "~/Content/view_source/search.css"));

            bundles.Add(new StyleBundle("~/Content/route/list").Include(
                        "~/Content/view_source/list.css"
                ));

            bundles.Add(new StyleBundle("~/Content/route/regandedit").Include(
                        "~/Content/view_source/regandedit.css"
                ));

            bundles.Add(new StyleBundle("~/Content/route/plaza").Include(
                        "~/Content/view_source/plaza.css"));

            bundles.Add(new StyleBundle("~/Content/route/aroundlist").Include(
                        "~/Content/view_source/aroundlist.css"));

            bundles.Add(new StyleBundle("~/Content/route/view").Include(
                        "~/Content/view_source/view.css"));

            bundles.Add(new StyleBundle("~/Content/route/searchresult").Include(
                        "~/Content/view_source/searchresult.css"
                ));

            bundles.Add(new StyleBundle("~/Content/route/yearcreate").Include(
                        "~/Content/view_source/yearcreate.css",
                        "~/Content/widget/calendarselect.css"));

            bundles.Add(new StyleBundle("~/Content/route/city").Include(
                        "~/Content/view_source/city.css"));

            bundles.Add(new StyleBundle("~/Content/route/confirm").Include(
                        "~/Content/widget/mobiscroll.core.css",
                        "~/Content/view_source/regandedit.css"));

            bundles.Add(new StyleBundle("~/Content/route/listview").Include(
                        "~/Content/view_source/search.css"));
            /* CSS END */

			#region "Bundles: Common"
			// Page: Common/WhatsNew
			bundles.Add(new ScriptBundle("~/bundles/common/navbar").Include("~/Scripts/view/navbar.js")); 
			#endregion


			//bundles.Add(new ScriptBundle("~/bundles/comment/comments").Include("~/Scripts/view/comments*")); 


			#region "Bundles: Insurance"
			// Page: Insurance/Create
			bundles.Add(new ScriptBundle("~/bundles/insurance/create").Include(
						 "~/Scripts/jquery-{version}.js",
						"~/Scripts/widget/mobipick.*",
						"~/Content/widget/template.js",
						"~/Scripts/view/insurance.js"));
			#endregion


            #region "Bundles: Order"
			// Page: Order/Create
			bundles.Add(new ScriptBundle("~/bundles/order/create").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/widget/mobipick.*",
						"~/Scripts/view/ordercreate.js"));

			// Page: Order/Detail
			bundles.Add(new ScriptBundle("~/bundles/order/detail").Include(
						"~/Scripts/view/orderdetail.js"));

			// Page: Order/List
			bundles.Add(new ScriptBundle("~/bundles/order/list").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Content/widget/template.js",
						"~/Scripts/view/orderlist.js"));

			bundles.Add(new ScriptBundle("~/bundles/order/edit").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/widget/mobipick.*",
						"~/Scripts/view/orderedit.js")); 
			#endregion
			

            #region "Bundles: Route"
			// Page: Route/Create
			bundles.Add(new ScriptBundle("~/bundles/route/maplocation").Include("~/Scripts/view/maplocation.js"));
			bundles.Add(new ScriptBundle("~/bundles/route/loadcreate").Include("~/Scripts/view/loadcreate.js"));
			bundles.Add(new ScriptBundle("~/bundles/route/loadsearch").Include("~/Scripts/view/loadsearch.js"));
			bundles.Add(new ScriptBundle("~/bundles/route/create").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/widget/mobipick.*",
						"~/Scripts/widget/convertor.js",
						"~/Scripts/view/routecreate.js"));

			// Page: Route/Detail
			bundles.Add(new ScriptBundle("~/bundles/route/detail").Include(
						"~/Content/widget/template.js",
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/view/common.js",
						"~/Scripts/view/routedetail.js"
			));

			// Page: Route/List
			bundles.Add(new ScriptBundle("~/bundles/route/list").Include(
						"~/Content/widget/template.js",
						"~/Scripts/view/routelist.js"));

			// Page: Route/Newest
			bundles.Add(new ScriptBundle("~/bundles/route/plaza").Include(
						"~/Content/widget/template.js",
						"~/Scripts/view/routeplaza.js"));

			// Page: Route/View
			bundles.Add(new ScriptBundle("~/bundles/route/view").Include(
						"~/Scripts/view/routeview.js",
						"~/Scripts/view/viewmap.js"));

			// Page: Route/DetailPeek
			//bundles.Add(new ScriptBundle("~/bundles/route/detailpeek").Include(
			//            "~/Scripts/jquery-{version}.js",
			//            "~/Scripts/view/peek.js",
			//            "~/Scripts/view/detailpeek.js",
			//            "~/Scripts/view/detailmap.js"));

			// Page: Route/Search
			bundles.Add(new ScriptBundle("~/bundles/route/search").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/widget/convertor.js",
						"~/Scripts/widget/mobipick.*",
						"~/Scripts/view/routesearch.js"));

			// Page: Route/SearchResult
			bundles.Add(new ScriptBundle("~/bundles/route/searchresult").Include(
						"~/Content/widget/template.js",
						"~/Scripts/view/routesearchresult.js"));

			// Page: Route/YearCreate
			//bundles.Add(new ScriptBundle("~/bundles/route/yearcreate").Include(
			//            "~/Scripts/jquery-1.10.2.js",
			//            "~/Scripts/widget/convertor.js",
			//            "~/Scripts/widget/moment.min.js",
			//            "~/Scripts/widget/calendar.moment.js",
			//            "~/Scripts/view/yearcreate.js"));

			// Page: Map/Create_Map
			bundles.Add(new ScriptBundle("~/bundles/map/createmap").Include(
						"~/Scripts/view/createmap.js"));

			// Page: Route/City
			bundles.Add(new ScriptBundle("~/bundles/route/city").Include(
						"~/Content/widget/template.js",
						"~/Scripts/view/city.js"));

			// Page: Route/Edit
			bundles.Add(new ScriptBundle("~/bundles/route/edit").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/widget/mobipick.*",
						"~/Scripts/view/routeedit.js")); 
			#endregion
            

            #region "Bundles: User"
			// Page: User/Arround
			bundles.Add(new ScriptBundle("~/bundles/user/around").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/view/around.js"));

			// Page: User/EditProfile
			bundles.Add(new ScriptBundle("~/bundles/user/editprofile").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/view/editprofile*"));

			// Page: User/ViewProfile
			bundles.Add(new ScriptBundle("~/bundles/user/viewprofile").Include(
						"~/Scripts/view/viewprofile*"));

			// Page: User/Register
			bundles.Add(new ScriptBundle("~/bundles/user/register").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/view/register.js"));

			// Page: User/Login
			bundles.Add(new ScriptBundle("~/bundles/user/login").Include(
						"~/Scripts/view/loginwindow.js"));

			// Page: User/Reset
			bundles.Add(new ScriptBundle("~/bundles/user/reset").Include(
						"~/Scripts/view/reset.js"));

			// Page: User/AroundList
			bundles.Add(new ScriptBundle("~/bundles/user/aroundlist").Include(
						"~/Content/widget/template.js",
						"~/Scripts/view/aroundlist.js"));

			// Page: User/AroundList2
			bundles.Add(new ScriptBundle("~/bundles/user/aroundlistlocation").Include("~/Scripts/view/baidu.js",
						"~/Scripts/widget/convertor.js",
						"~/Scripts/view/aroundlistlocation.js"));
			#endregion
        }
    }
}
