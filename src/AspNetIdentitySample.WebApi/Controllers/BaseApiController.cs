using AspNetIdentitySample.WebApi.Identity;
using AspNetIdentitySample.WebApi.Models;
using Exceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentitySample.WebApi.Controllers
{
    public class BaseApiController : ApiController
    {

        private ModelFactory _modelFactory;
        private ApplicationUserManager _AppUserManager = null;

        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return _AppUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _AppUserManager = value;
            }
        }


        private ApplicationSignInManager _SignInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _SignInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _SignInManager = value;
            }
        }


        public BaseApiController()
        {
        }

        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request, this.AppUserManager);
                }
                return _modelFactory;
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
        protected IHttpActionResult GetErrorInvalidApiRequestException(InvalidApiRequestException ex)
        {
            if (ex == null)
            {
                return InternalServerError();
            }

            if (ex.Errors != null)
            {
                foreach (string error in ex.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return BadRequest(ModelState);
        }
        protected IHttpActionResult GetError(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                return InternalServerError();
            }

            ModelState.AddModelError("", error);

            return BadRequest(ModelState);
        }

    }
}
