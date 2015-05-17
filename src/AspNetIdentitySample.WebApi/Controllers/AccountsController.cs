using AspNetIdentitySample.WebApi.Identity;
using AspNetIdentitySample.WebApi.Models;
using Exceptions;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;


namespace AspNetIdentitySample.WebApi.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        public AccountsController()
        {
        }
        public AccountsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            AppUserManager = userManager;
            SignInManager = signInManager;
        }
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (addUserResult.Succeeded)
            {
                var code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code })); ;
                try
                {
                    await this.AppUserManager.SendEmailAsync(user.Id, "Confirme sua Conta", "Por favor confirme sua conta clicando neste link: <a href='" + callbackUrl + "'>Confirmar Cadastro</a>");
                }
                catch (InvalidApiRequestException ex)
                {
                    return GetErrorInvalidApiRequestException(ex);
                }
            }
            else
            {
                return GetErrorResult(addUserResult);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id & Code são requeridos");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var user = await AppUserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Não revelar se o usuario nao existe ou nao esta confirmado
                return GetError("Usuario / Senha não coincidem");
            }
            var result = await AppUserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {

            //Only SuperAdmin or Admin can delete users (Later when implement roles)

            var appUser = await AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                
                IdentityResult result = await AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }
    }
}