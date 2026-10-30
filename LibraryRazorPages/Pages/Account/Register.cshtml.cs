using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DTO;
using LibraryRazorPages.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LibraryRazorPages.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        public RegisterModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        } 

        [BindProperty]
        public RegisterFormDto RegisterForm { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existingUser = await _userRepository.ExistsAsync(RegisterForm.Login);
            if (existingUser)
            {
                ModelState.AddModelError("RegisterForm.Login", "Пользователь с таким логином уже существует");
                return Page();
            }

            var passwordHashToStore = GetPasswordHash();
            var user = new User(0, RegisterForm.Login, passwordHashToStore, "User");
            var userId = await _userRepository.CreateAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Index");
        }

        private string GetPasswordHash()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: RegisterForm.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            return $"{Convert.ToBase64String(salt)}:{hashedPassword}";
        }
    }
}
