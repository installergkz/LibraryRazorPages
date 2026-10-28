using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LibraryRazorPages.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public LoginModel(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        } 

        [BindProperty]
        public LoginInput Input { get; set; }
        public string ErrorMessage { get; set; }

        public class LoginInput
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Input.Username) || string.IsNullOrEmpty(Input.Password))
            {
                ErrorMessage = "Введите логин и пароль";
                return Page();
            }

            using var connection = _connectionFactory.CreateConnection();

            var user = await connection.QuerySingleOrDefaultAsync<UserDbModel>(
                Queries.StoredProcedures.GetUserByName,
                new { Input.Username },
                commandType: CommandType.StoredProcedure);

            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                return Page();
            }

            // Проверяем пароль
            if (!VerifyPassword(Input.Password, user.PasswordHash))
            {
                ErrorMessage = "Неверный логин или пароль";
                return Page();
            }

            // Создаем куки
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Index");
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split(':');
                if (parts.Length != 2) return false;

                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);

                var testHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000, 
                    numBytesRequested: 32);

                return CryptographicOperations.FixedTimeEquals(hash, testHash);
            }
            catch
            {
                return false;
            }
        }

        private class UserDbModel
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string PasswordHash { get; set; }
            public string Role { get; set; }
        }
    }
}