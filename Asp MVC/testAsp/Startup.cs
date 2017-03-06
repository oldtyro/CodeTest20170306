using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(testAsp.Startup))]
namespace testAsp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
