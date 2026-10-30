using LibraryRazorPages.Enums;
using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace LibraryRazorPages.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IMongoLogService _mongoLog;

        public DeleteModel(IBookService bookService, IMongoLogService mongoLog)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _mongoLog = mongoLog ?? throw new ArgumentNullException(nameof(mongoLog)); ;
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var book = await _bookService.GetAsync(id);
            if (book is null)
                return NotFound();

            var deleted = await _bookService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            TempData["StatusMessage"] = "Книга удалена.";

            int? userId = null;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var parsed))
                userId = parsed;

            await _mongoLog.LogAsync("Book", book.Id.ToString(), book.Name, LogAction.Delete, userId);
            return RedirectToPage("./Index");
        }
    }
}
