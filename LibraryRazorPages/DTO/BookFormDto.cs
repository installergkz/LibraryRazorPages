using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryRazorPages.DTO
{
    public record BookFormDto
    {
        public int Id { get; init; }

        [Required(ErrorMessage = "Укажите название")]
        [MaxLength(500)]
        [Display(Name = "Название")]
        public string? Name { get; init; }

        [Required(ErrorMessage = "Укажите авторов")]
        [MaxLength(500)]
        [Display(Name = "Авторы")]
        public string? Authors { get; init; }

        [Range(1800, 2100, ErrorMessage = "Год должен быть от 1800 до 2100")]
        [Display(Name = "Год издания")]
        public int? PublicationYear { get; init; }

        [Display(Name = "Издательство")]
        public int? PublicationHouseId { get; init; }

        [Range(1, 100000, ErrorMessage = "Некорректное число страниц")]
        [Display(Name = "Страниц")]
        public int? PagesCount { get; init; }

        [Display(Name = "ISBN")]
        [RegularExpression(@"^(?:\d{9}[\dX]|\d{1,5}-\d{1,7}-\d{1,7}-[\dX]|97[89]\d{10}|97[89]-\d{1,5}-\d{1,7}-\d{1,7}-\d)$", ErrorMessage = "Некорректный ISBN")]
        public string? ISBN { get; init; }

        [MaxLength(1000)]
        [Display(Name = "Примечание")]
        public string? Note { get; init; }

        [MaxLength(10000)]
        [Display(Name = "Оглавление")]
        public string? ContentsList { get; init; }

        public IEnumerable<SelectListItem> PublicationHouses { get; init; } = Array.Empty<SelectListItem>();
    }
}