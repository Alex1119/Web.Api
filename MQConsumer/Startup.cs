using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MQConsumer.Startup))]
namespace MQConsumer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
