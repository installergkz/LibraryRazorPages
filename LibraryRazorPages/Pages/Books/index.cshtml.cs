using LibraryRazorPages.Models;
using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryRazorPages.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;

        public IndexModel(IBookService bookService)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }

        public IReadOnlyList<Book> Books { get; set; } = Array.Empty<Book>();

        public async Task OnGetAsync(CancellationToken ct = default)
        {
            Books = await _bookService.GetAllAsync(ct) ?? Array.Empty<Book>();
        }
    }
}