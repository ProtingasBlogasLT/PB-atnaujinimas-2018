using System.Threading.Tasks;
using PB.WebAPI.Models;

namespace PB.WebAPI.Repositories
{
    public interface IUsersRepo : IRepo<User>
    {
        Task<User> ReadAsync(string username);
    }
}