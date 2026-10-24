using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryRazorPages.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly IBookService _bookService;

        public DeleteModel(IBookService bookService)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var deleted = await _bookService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            TempData["StatusMessage"] = "Книга удалена.";
            return RedirectToPage("./Index");
        }
    }
}
