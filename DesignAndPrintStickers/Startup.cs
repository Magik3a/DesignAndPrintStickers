using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DesignAndPrintStickers.Startup))]
namespace DesignAndPrintStickers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
