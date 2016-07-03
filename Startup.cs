using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using PassPerfectWebAPI.App_Start;
using PassPerfectWebAPI.Domain.Context;
using PassPerfectWebAPI.Domain.Infrastructure;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace PassPerfectWebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);
            ConfigureWebApi(config);
            app.UseCors(CorsOptions.AllowAll);
            AutofacConfig.BootstrapAutofac(app, config);
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            app.CreatePerOwinContext(PassPerfectDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            var OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["Issuer"])
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["Issuer"];
            string audienceId = ConfigurationManager.AppSettings["AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }
    }
}