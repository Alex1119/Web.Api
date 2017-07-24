using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Mvc;

namespace WEB.API.App_Start
{
    public class AutoFacConfig
    {
        public static IContainer AutoFacInit()
        {
            //APIController 中使用属性注入  逻辑层与数据层使用构造方法注入

            var builder = new ContainerBuilder();
            HttpConfiguration config = GlobalConfiguration.Configuration;

            SetupResolveRules(builder);

            ////注册所有的Controllers
            //builder.RegisterControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            //注册所有的ApiControllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();

            var container = builder.Build();
            //注册api容器需要使用HttpConfiguration对象
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            return container;
        }

        private static void SetupResolveRules(ContainerBuilder builder)
        {
            ////WebAPI只用引用services和repository的接口，不用引用实现的dll。
            ////如需加载实现的程序集，将dll拷贝到bin目录下即可，不用引用dll
            //var iRepository = Assembly.Load("WebApi.IRepository");
            //var iServices = Assembly.Load("WebApi.IServices");
            //var repository = Assembly.Load("WebApi.Repostory");
            //var services = Assembly.Load("WebApi.Services");

            //用GetReferencedAssemblies方法获取当前应用程序下所有的程序集
            var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>();

            //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            builder.RegisterAssemblyTypes(assemblys.ToArray())
              .Where(t => t.Name.EndsWith("Service"))
              .AsImplementedInterfaces()
              .InstancePerLifetimeScope(); //一次请求内单例

            //根据名称约定（数据访问层的接口和实现均以Repository结尾），实现数据访问接口和数据访问实现的依赖
            builder.RegisterAssemblyTypes(assemblys.ToArray())
              .Where(t => t.Name.EndsWith("Repository"))
              .AsImplementedInterfaces()
              .InstancePerLifetimeScope(); //一次请求内单例

        }

        //private static void SetupResolveRules(ContainerBuilder builder)
        //{
        //    //builder.RegisterType<PersonRepository>().As<IPersonRepository>();
        //    builder.RegisterType<PersonServices>().As<IPersonServices>();

        //    builder.RegisterType<PersonRepository>().Named<IPersonRepository>("PersonServices1");
        //    builder.RegisterType<PersonRepository>().Named<IPersonRepository>("PersonServices2");
        //}
    }
}