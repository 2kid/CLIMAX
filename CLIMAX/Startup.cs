using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CLIMAX.Startup))]
namespace CLIMAX
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
