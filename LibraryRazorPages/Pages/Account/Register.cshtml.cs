using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LibraryRazorPages.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public RegisterModel(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        } 

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Логин обязателен")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            using var connection = _connectionFactory.CreateConnection();

            var existingUser = await connection.ExecuteScalarAsync<bool>(
                Queries.StoredProcedures.CheckUserExists, 
                new { Input.Username }, 
                commandType: CommandType.StoredProcedure);
            
            if (existingUser)
            {
                ModelState.AddModelError("Input.Username", "Пользователь с таким логином уже существует");
                return Page();
            }

            // Хэшируем пароль
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Input.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            string passwordHashToStore = $"{Convert.ToBase64String(salt)}:{hashedPassword}";

            await connection.ExecuteAsync(
                Queries.StoredProcedures.CreateUser,
                new { Input.Username, PasswordHash = passwordHashToStore, Role = "User" },
                commandType: CommandType.StoredProcedure);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Input.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Index");
        }
    }
}
