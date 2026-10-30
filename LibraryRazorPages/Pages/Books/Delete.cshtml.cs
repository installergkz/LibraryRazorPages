using LibraryRazorPages.Enums;
using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            await _mongoLog.LogAsync("Book", book.Id.ToString(), book.Name, LogAction.Delete, User.Identity?.Name);
            return RedirectToPage("./Index");
        }
    }
}
