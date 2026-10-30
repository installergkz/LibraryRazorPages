using LibraryRazorPages.DTO;
using LibraryRazorPages.Models;

namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string login, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateRoleAsync(int id, string role, string currentUserLogin, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(User user, CancellationToken cancellationToken = default);
    }
}
