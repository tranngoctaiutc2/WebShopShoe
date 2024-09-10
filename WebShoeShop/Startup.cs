using Microsoft.Owin;
using Owin;
using System.Web.Services.Description;

[assembly: OwinStartupAttribute(typeof(WebShoeShop.Startup))]
namespace WebShoeShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            
        }

    }
}
