using System.ComponentModel.DataAnnotations;

namespace LibraryRazorPages.DTO
{
    public record RegisterFormDto
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
        public string Login { get; init; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 20 символов")]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; init; }
    }
}
