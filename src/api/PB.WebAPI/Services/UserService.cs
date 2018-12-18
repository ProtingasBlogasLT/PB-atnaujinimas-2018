using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PB.WebAPI.Models;
using PB.WebAPI.Repositories;

namespace PB.WebAPI.Services
{
    public class UserService : IUserService
    {
        public ITokenService TokenService { get; }
        private IUsersRepo UsersRepo { get; }
        private IPasswordService PasswordService { get; }

        public UserService(ITokenService tokenService, IUsersRepo usersRepo, IPasswordService passwordService)
        {
            TokenService = tokenService;
            UsersRepo = usersRepo;
            PasswordService = passwordService;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await UsersRepo.ReadAsync(username);
            if (user == null)
            {
                return null;
            }

            
            var success = PasswordService.ValidatePassword(password, user.HashedPassword);
            password = null;

            if (!success)
            {
                return null;
            }

            user.Token = TokenService.GenerateToken(user.Id);
            user.Password = null;
            user.HashedPassword = null;

            return user;
        }

        public async Task CreateUserAsync(User user)
        {
            user.HashedPassword = PasswordService.HashPassword(user.Password);
            user.Password = null;
            await UsersRepo.CreateAsync(user);
            user.HashedPassword = null;
        }
    }
}
