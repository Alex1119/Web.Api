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
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var Container = AutoFacConfig.AutoFacInit();
            var userConsumer = Container.Resolve<UserConsumer>();
            var userConsumer2 = Container.Resolve<UserConsumer2>();
            userConsumer.Sub();
            userConsumer2.Sub();
        }
    }
}
