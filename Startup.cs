using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Shortly.Startup))]
namespace Shortly
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
