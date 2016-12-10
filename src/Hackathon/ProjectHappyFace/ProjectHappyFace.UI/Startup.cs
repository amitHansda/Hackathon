using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectHappyFace.UI.Startup))]
namespace ProjectHappyFace.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
