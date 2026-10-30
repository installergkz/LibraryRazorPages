using LibraryRazorPages.DTO;
using LibraryRazorPages.Enums;
using LibraryRazorPages.Models;
using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryRazorPages.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IPublicationHouseService _pubHouseService;
        private readonly IMongoLogService _mongoLog;

        public CreateModel(IBookService bookService, IPublicationHouseService pubHouseService, IMongoLogService mongoLog)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _pubHouseService = pubHouseService ?? throw new ArgumentNullException(nameof(pubHouseService));
            _mongoLog = mongoLog ?? throw new ArgumentNullException(nameof(mongoLog));
        }

        [BindProperty]
        public BookFormDto BookForm { get; set; } = new();

        public async Task OnGetAsync()
        {
            BookForm = BookForm with { PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync() };
        } 

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                BookForm = BookForm with { PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync() };
                return Page();
            }

            try
            {
                var id = await _bookService.CreateAsync(BookMapper.ToBook(BookForm));
                TempData["StatusMessage"] = $"Книга «{BookForm.Name}» создана.";
                await _mongoLog.LogAsync("Book", id.ToString(), BookForm.Name, LogAction.Create, User.Identity?.Name);
                return RedirectToPage("./Details", new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                BookForm = BookForm with { PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync() };
                return Page();
            }
        }
    }
}
