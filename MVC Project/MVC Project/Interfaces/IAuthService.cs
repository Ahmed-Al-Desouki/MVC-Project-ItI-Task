using MVC_Project.Models;
using System.Threading.Tasks;

namespace MVC_Project.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);
        Task<bool> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        bool IsAuthenticated();
    }
}