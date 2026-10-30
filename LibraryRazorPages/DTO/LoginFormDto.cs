using System.ComponentModel.DataAnnotations;

namespace LibraryRazorPages.DTO
{
    public record LoginFormDto
    {
        [Required(ErrorMessage = "Логин обязателен")]
        public string Login { get; init; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; init; }
    }
}
