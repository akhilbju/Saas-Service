using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Saas_Auth_Service.Controller
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController
    {
        /// <summary>
        /// Jwt Settings Service
        /// </summary>
        private readonly IJwtSettings _jwtSettings;

        /// <summary>
        /// Password Hasher Service
        /// </summary>
        private readonly IPasswordHasherService _passwordHasherService;

        /// <summary>
        /// Database Context
        /// </summary>
        private readonly AppDbContext _dbContext;

        public AuthController(IJwtSettings jwtSettings, IPasswordHasherService passwordHasherService, AppDbContext dbContext)
        {
            _jwtSettings = jwtSettings;
            _passwordHasherService = passwordHasherService;
            _dbContext = dbContext;
        }

        #region  public methods

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public Response CreateUser(CreateUser request)
        {
            Response res = new();
            User newUser = new()
            {
                Username = request.Username,
                PasswordHash = _passwordHasherService.Hash(request.Password),
                Email = request.Email,
                UserType = request.UserType
            };
            this._dbContext.Users.Add(newUser);
            this._dbContext.SaveChanges();
            res.IsSuccess = true;
            res.Message = "User created successfully";
            return res;
        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public LoginResponse LoginUser(LoginUser request)
        {
            LoginResponse res = new();
            var user = this._dbContext.Users.Where(u => u.Email == request.Email).FirstOrDefault();
            if (user == null)
            {
                res.IsSuccess = false;
                res.Message = ErrorMessages.UserNotFound;
                return res;
            }
            if (!_passwordHasherService.Verify(request.Password, user.PasswordHash))
            {
                res.IsSuccess = false;
                res.Message = ErrorMessages.InvalidCredentials;
                return res;
            }
            var token = _jwtSettings.GenerateToken(user);
            res.IsSuccess = true;
            res.AccessToken = token;
            res.RefreshToken = Guid.NewGuid().ToString(); 
            createRefreshTokenForUser(user.Id, res.RefreshToken);
            res.UserType = user.UserType;
            res.Message = SuccessMessages.LoginSuccess;
            return res;
        }

        /// <summary>
        /// Generate Refresh Token
        /// </summary>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<LoginResponse> GenerateRefreshToken(string RefreshToken)
        {
            LoginResponse res = new();
            var refreshToken = this._dbContext.RefreshTokens.Where(r => r.Token == RefreshToken && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow).FirstOrDefault();
            if (refreshToken == null)
            {
                res.IsSuccess = false;
                res.Message = ErrorMessages.TokenExpired;
                return res;
            }
            var user = this._dbContext.Users.Where(u => u.Id == refreshToken.UserId).FirstOrDefault();
            if (user == null)
            {
                res.IsSuccess = false;
                res.Message = ErrorMessages.UserNotFound;
                return res;
            }
            refreshToken.IsRevoked = true;
            this._dbContext.RefreshTokens.Update(refreshToken);
            var newAccessToken = _jwtSettings.GenerateToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();
            createRefreshTokenForUser(user.Id, newRefreshToken);
            res.IsSuccess = true;
            res.AccessToken = newAccessToken;
            res.RefreshToken = newRefreshToken;
            return res;
        }
        #endregion

        /// <summary>
        /// Create Refresh Token for User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshToken"></param>
        #region private methods
        private void createRefreshTokenForUser(int userId, string refreshToken)
        {
            RefreshToken newRefreshToken = new RefreshToken()
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            _dbContext.RefreshTokens.Add(newRefreshToken);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}