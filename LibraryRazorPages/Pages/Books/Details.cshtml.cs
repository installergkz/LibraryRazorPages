using LibraryRazorPages.DTO;
using LibraryRazorPages.Enums;
using LibraryRazorPages.Models;
using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace LibraryRazorPages.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IPublicationHouseService _pubHouseService;
        private readonly IMongoLogService _mongoLog;

        public DetailsModel(IBookService bookService, IPublicationHouseService pubHouseService, IMongoLogService mongoLog)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _pubHouseService = pubHouseService ?? throw new ArgumentNullException(nameof(pubHouseService));
            _mongoLog = mongoLog ?? throw new ArgumentNullException(nameof(mongoLog)); ;
        }

        [BindProperty]
        public BookFormDto BookForm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, bool isUpdated = false)
        {
            var book = await _bookService.GetAsync(id);
            if (book is null)
                return NotFound();

            BookForm = BookMapper.ToBookForm(book);
            BookForm = BookForm with { PublicationHouses = await _pubHouseService.LoadPublicationHousesAsync() };

            if (!isUpdated)
            {
                int? userId = null;
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var parsed))
                    userId = parsed;

                await _mongoLog.LogAsync("Book", book.Id.ToString(), book.Name, LogAction.View, userId);
            }

            return Page();
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
                var updated = await _bookService.UpdateAsync(BookMapper.ToBook(BookForm));
                if (!updated)
                    return NotFound();

                TempData["StatusMessage"] = $"Книга «{BookForm.Name}» сохранена.";

                int? userId = null;
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var parsed))
                    userId = parsed;

                await _mongoLog.LogAsync("Book", BookForm.Id.ToString(), BookForm.Name, LogAction.Update, userId);
                return RedirectToPage("./Details", new { id = BookForm.Id, isUpdated = true });
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
