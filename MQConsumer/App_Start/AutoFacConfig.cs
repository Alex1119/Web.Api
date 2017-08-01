using Autofac;
using Autofac.Integration.Mvc;
using Service.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;

namespace MQConsumer.App_Start
{
    public class AutoFacConfig
    {
        public static IContainer AutoFacInit()
        {
            var builder = new ContainerBuilder();
            SetupResolveRules(builder);
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            return container;
        }

        private static void SetupResolveRules(ContainerBuilder builder)
        {
            //用GetReferencedAssemblies方法获取当前应用程序下所有的程序集
            var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>();

            //根据名称约定（服务层的接口和实现均以BLL结尾），实现服务接口和服务实现的依赖
            builder.RegisterAssemblyTypes(assemblys.ToArray())
              .Where(t => t.Name.EndsWith("Service"))
              .AsImplementedInterfaces();

            //根据名称约定（数据访问层的接口和实现均以Service结尾），实现数据访问接口和数据访问实现的依赖
            builder.RegisterAssemblyTypes(assemblys.ToArray())
              .Where(t => t.Name.EndsWith("Repository"))
              .AsImplementedInterfaces();

            builder.RegisterType<UserConsumer>().SingleInstance();
            builder.RegisterType<DLXConsumer>().SingleInstance();

        }
    }
}