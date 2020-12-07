using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TruckingVSM.Startup))]
namespace TruckingVSM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
