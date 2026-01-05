using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Saas_Auth_Service.Controller
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authServices)
        {          
            _authServices = authServices;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public Response CreateUser(CreateUser request)
        {
           return _authServices.Register(request);
        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public LoginResponse LoginUser(LoginUser request)
        {
            return _authServices.Login(request);
        }

        /// <summary>
        /// Generate Refresh Token
        /// </summary>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<LoginResponse> GenerateRefreshToken(string RefreshToken)
        {
            return await _authServices.GenerateRefreshToken(RefreshToken);
        }
    }
}