using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using PassPerfectWebAPI.Domain.Interfaces;
using PassPerfectWebAPI.Domain.Repository;
using PassPerfectWebAPI.Services;
using PassPerfectWebAPI.Services.Interfaces;
using System.Reflection;
using System.Web.Http;

namespace PassPerfectWebAPI.App_Start
{
    public static class AutofacConfig
    {
        public static void BootstrapAutofac(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
       
            //Register Controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<PasswordService>().As<IPasswordService>().InstancePerRequest();
            builder.RegisterType<HashService>().As<IHashService>().InstancePerRequest();
            builder.RegisterType<PGPService>().As<IPGPService>().InstancePerRequest();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }
    }
}