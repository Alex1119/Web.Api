using Autofac;
using MQConsumer.App_Start;
using Service.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MQConsumer
{
    public class MvcApplication : HttpApplication
    {
        public static IContainer Container { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Container = AutoFacConfig.AutoFacInit();
            var userConsumer = Container.Resolve<UserConsumer>();
            var dlxConsumer = Container.Resolve<DLXConsumer>();
            userConsumer.Sub();
            dlxConsumer.Sub();
            //new DLXProducter().Declare();
        }
    }
}
