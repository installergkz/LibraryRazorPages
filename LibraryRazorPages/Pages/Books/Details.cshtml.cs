using LibraryRazorPages.Models.ViewModels;
using LibraryRazorPages.Services;
using LibraryRazorPages.Services.Interfaces;
using LibraryRazorPages.Services.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryRazorPages.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IPublicationHouseService _pubHouseService;
        private readonly MongoLogService _mongoLog;

        public DetailsModel(IBookService bookService, IPublicationHouseService pubHouseService, MongoLogService mongoLog)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _pubHouseService = pubHouseService ?? throw new ArgumentNullException(nameof(pubHouseService));
            _mongoLog = mongoLog;
        }

        [BindProperty]
        public BookForm BookForm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var book = await _bookService.GetAsync(id);
            if (book is null)
                return NotFound();

            BookForm = BookMapper.ToBookForm(book);
            BookForm.PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync();
            await _mongoLog.LogViewAsync(book.Name, User.Identity?.Name);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                BookForm.PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync();
                return Page();
            }

            try
            {
                var updated = await _bookService.UpdateAsync(BookMapper.ToBook(BookForm));
                if (!updated)
                    return NotFound();

                TempData["StatusMessage"] = $"Книга «{BookForm.Name}» сохранена.";
                return RedirectToPage("./Details", new { id = BookForm.Id });
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
