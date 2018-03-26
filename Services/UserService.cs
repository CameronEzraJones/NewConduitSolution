using Conduit.Context;
using Conduit.Exceptions;
using Conduit.Model;
using Conduit.Model.DAO;
using Conduit.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Conduit.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ConduitDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IUserPersonalizationRepository _userPersonalizationRepository;

        public UserService(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration config,
            ConduitDbContext context,
            ILoggerFactory loggerFactory,
            IUserPersonalizationRepository userPersonalizationRepository
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _context = context;
            _logger = loggerFactory.CreateLogger<UserService>();
            _userPersonalizationRepository = userPersonalizationRepository;
        }

        internal override async Task<User> AuthenticateUser(string email, string password)
        {
            IdentityUser identityUser = await _userManager.FindByEmailAsync(email);
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, false);
            if (!result.Succeeded)
            {
                throw new ConduitUnauthorizedException($"Failed to sign in {email}");
            }
            UserPersonalizationDAO userPersonalization = _userPersonalizationRepository.GetUserPersonalization(identityUser.Id);
            return new User(identityUser.Email, GenerateJWTToken(identityUser), identityUser.UserName, userPersonalization.Bio, userPersonalization.Image);
        }

        private string GenerateJWTToken(IdentityUser identityUser)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal override async Task<User> GetCurrentUser(string username)
        {
            IdentityUser identityUser = await _userManager.FindByNameAsync(username);
            if (null == identityUser)
            {
                throw new ConduitNotFoundException($"Unable to find user with username {username}");
            }
            UserPersonalizationDAO userPersonalization = _userPersonalizationRepository.GetUserPersonalization(identityUser.Id);
            return new User(identityUser.Email, GenerateJWTToken(identityUser), identityUser.UserName, userPersonalization.Bio, userPersonalization.Image);
        }

        internal override async Task<User> RegisterUser(string username, string email, string password)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                IdentityUser user = new IdentityUser { UserName = username, Email = email };
                IdentityResult result = await _userManager.CreateAsync(user, password);
                if(!result.Succeeded)
                {
                    List<String> errors = new List<String>();
                    foreach(var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }
                    throw new ConduitValidationException(errors);
                }
                _userPersonalizationRepository.CreateUserPersonalizationForId(user.Id);
                transaction.Commit();
                return new User(email, GenerateJWTToken(user), username, null, null);
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        internal override async Task<User> UpdateUser(string username, UserUpdateData updateData)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                IdentityUser identityUser = await _userManager.FindByNameAsync(username);
                if(null != updateData.Username)
                {
                    var result = await _userManager.SetUserNameAsync(identityUser, updateData.Username);
                    if(!result.Succeeded)
                    {
                        throw new ConduitServerException($"Failed to update username of user with username {username}");
                    }
                }
                if(null != updateData.Email)
                {
                    var result = await _userManager.SetUserNameAsync(identityUser, updateData.Username);
                    if(!result.Succeeded)
                    {
                        throw new ConduitServerException($"Failed to update email of user with username {username}");
                    }
                }
                if(null != updateData.Password)
                {
                    var passwordHash = _userManager.PasswordHasher.HashPassword(identityUser, updateData.Password);
                    identityUser.PasswordHash = passwordHash;
                    var result = await _userManager.UpdateAsync(identityUser);
                    if(!result.Succeeded)
                    {
                        throw new ConduitServerException($"Failed to update password of user with username {username}");
                    }
                }
                if(null != updateData.Bio)
                {
                    _userPersonalizationRepository.UpdateUserBio(identityUser.Id, updateData.Bio);
                }
                if(null != updateData.Image)
                {
                    _userPersonalizationRepository.UpdateUserImage(identityUser.Id, updateData.Image);
                }
                _context.SaveChanges();
                transaction.Commit();
                UserPersonalizationDAO userPersonalizationDTO = _userPersonalizationRepository.GetUserPersonalization(identityUser.Id);
                return new User(identityUser.Email, GenerateJWTToken(identityUser), identityUser.UserName, userPersonalizationDTO.Bio, userPersonalizationDTO.Image);
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
    }
}
