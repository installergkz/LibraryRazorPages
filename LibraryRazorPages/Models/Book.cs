namespace LibraryRazorPages.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Authors { get; set; }
        public int? PublicationYear { get; set; }
        public PublicationHouse? PublicationHouse { get; set; }
        public int? PagesCount { get; set; }
        public string? ISBN { get; set; }
        public string? Note { get; set; }
        public string? ContentsList { get; set; }
    }
}