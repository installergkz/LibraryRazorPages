using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryRazorPages.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public UsersModel(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
        }

        public IEnumerable<UserDto> Users { get; set; }

        public async Task OnGetAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            Users = await connection.QueryAsync<UserDto>(Queries.StoredProcedures.GetUsers, commandType: CommandType.StoredProcedure);
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(int userId, string newRole)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(
                    Queries.StoredProcedures.UpdateUserRole,
                    new
                    {
                        Id = userId,
                        Role = newRole,
                        CurrentUsername = User.Identity?.Name
                    },
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected == 0)
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
