using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace LibraryRazorPages.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        public UsersModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IReadOnlyList<UserDto> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userRepository.GetAllAsync();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(int userId, string newRole)
        {
            try
            {
                var hasRowsAffected = await _userRepository.UpdateRoleAsync(userId, newRole, User.Identity?.Name);
                if (!hasRowsAffected)
                {
                    TempData["ErrorMessage"] = "Пользователь не найден.";
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage();
        }
    }
}
