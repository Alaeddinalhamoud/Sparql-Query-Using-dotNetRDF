using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RDFMVC.Startup))]
namespace RDFMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
