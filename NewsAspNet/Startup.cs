using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NewsAspNet.Startup))]
namespace NewsAspNet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
