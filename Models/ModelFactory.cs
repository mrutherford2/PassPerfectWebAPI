using PassPerfectWebAPI.Domain.Infrastructure;
using System.Net.Http;
using System.Web.Http.Routing;

namespace PassPerfectWebAPI.Models
{
    public class ModelFactory
    {
        private UrlHelper UrlHelper;
        private ApplicationUserManager AppUserManager; 

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager appUserManager)
        {
            UrlHelper = new UrlHelper(request);
            AppUserManager = appUserManager; 
        }

        public UserReturnModel Create(ApplicationUser appUser)
        {
            return new UserReturnModel
            {
                Url = UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName),
                Email = appUser.Email,
                EmailConfirmed = appUser.EmailConfirmed,
                JoinDate = appUser.JoinDate,
                Roles = AppUserManager.GetRolesAsync(appUser.Id).Result,
                Claims = AppUserManager.GetClaimsAsync(appUser.Id).Result,
                PublicKeyPath = appUser.PublicKeyPath,
                OTPHash = appUser.OTPHash
            };
        }
    }
}