using PassPerfectWebAPI.Domain.Infrastructure;
using PassPerfectWebAPI.Models;
using PassPerfectWebAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace PassPerfectWebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        private readonly IPasswordService passwordService;
        private readonly IHashService hashService;
        private readonly IPGPService pgpService;

        public AccountsController(
            IPasswordService passwordService,
            IHashService hashService,
            IPGPService pgpService)
        {
            this.passwordService = passwordService;
            this.hashService = hashService;
            this.pgpService = pgpService;
        }

        /// <summary>
        /// Generates a One Time Password encrypted with the users PGP Public Key
        /// </summary>
        /// <returns>The encrypted OTP</returns>
        [Route("onetimepassword/{username}")]
        public async Task<IHttpActionResult> GetOneTimePass(string username)
        {
            var user = await AppUserManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            var model = HydrateReturnModel(user, username);
            return Ok(model.CipherText);
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);
            if (user != null)
            {
                return Ok(this.ModelFactory.Create(user));
            }
            return NotFound();
        }

        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);
            if (user != null)
            {
                return Ok(this.ModelFactory.Create(user));
            }
            return NotFound();
        }

        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                JoinDate = DateTime.Now
            };

            var addUserResult = await AppUserManager.CreateAsync(user, model.Password);
            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }
            var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));
            return Created(locationHeader, ModelFactory.Create(user));
        }

        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginModel model)
        {
            var user = await AppUserManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return NotFound();
            }
            if(hashService.VerifyHash(model.Password, user))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        private TestModel HydrateReturnModel(ApplicationUser user, string username)
        {
            var model = new TestModel();
            model.GeneratedPassword = passwordService.GetRandomPassword();
            hashService.StringToHash(model.GeneratedPassword, user, true);
            model.CipherText = pgpService.GetEncryptPassword(model.GeneratedPassword, user, username);
            string cleanedCipherText = model.CipherText.Replace("\r\n", "<br/>");
            model.CipherText = cleanedCipherText;                                                                                           
            return model;
        }
    }
}