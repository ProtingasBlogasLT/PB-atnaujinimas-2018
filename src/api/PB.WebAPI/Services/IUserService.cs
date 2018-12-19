using System.Threading.Tasks;
using PB.WebAPI.Models;

namespace PB.WebAPI.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task CreateUserAsync(User user);
    }
}