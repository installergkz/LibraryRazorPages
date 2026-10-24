using LibraryRazorPages.Models.ViewModels;
using LibraryRazorPages.Services.Interfaces;
using LibraryRazorPages.Services.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryRazorPages.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IPublicationHouseService _pubHouseService;

        public CreateModel(IBookService bookService, IPublicationHouseService pubHouseService)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _pubHouseService = pubHouseService ?? throw new ArgumentNullException(nameof(pubHouseService));
        }

        [BindProperty]
        public BookForm BookForm { get; set; } = new();

        public async Task OnGetAsync() => BookForm.PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                BookForm.PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync();
                return Page();
            }

            try
            {
                var id = await _bookService.CreateAsync(BookMapper.ToBook(BookForm));
                TempData["StatusMessage"] = $"Книга «{BookForm.Name}» создана.";
                return RedirectToPage("./Details", new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                BookForm.PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync();
                return Page();
            }
        }
    }
}
