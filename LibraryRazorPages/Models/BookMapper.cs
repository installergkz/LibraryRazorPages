using LibraryRazorPages.DTO;

namespace LibraryRazorPages.Models
{
    public static class BookMapper
    {
        public static Book ToBook(BookFormDto bookForm) => new Book
        {
            Id = bookForm.Id,
            Name = bookForm.Name,
            Authors = bookForm.Authors,
            PublicationYear = bookForm.PublicationYear,
            PublicationHouse = bookForm.PublicationHouseId.HasValue ? new PublicationHouse { Id = bookForm.PublicationHouseId.Value } : null,
            PagesCount = bookForm.PagesCount,
            ISBN = bookForm.ISBN,
            Note = bookForm.Note,
            ContentsList = bookForm.ContentsList
        };

        public static BookFormDto ToBookForm(Book book) => new BookFormDto
        {
            Id = book.Id,
            Name = book.Name,
            Authors = book.Authors,
            PublicationYear = book.PublicationYear,
            PublicationHouseId = book.PublicationHouse?.Id,
            PagesCount = book.PagesCount,
            ISBN = book.ISBN,
            Note = book.Note,
            ContentsList = book.ContentsList
        };
    }
}