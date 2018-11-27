using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookWyrm.Web.Startup))]
namespace BookWyrm.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
