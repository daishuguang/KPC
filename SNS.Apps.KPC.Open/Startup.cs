using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SNS.Apps.KPC.Open.Startup))]
namespace SNS.Apps.KPC.Open
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
