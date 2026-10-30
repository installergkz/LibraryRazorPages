namespace LibraryRazorPages.Models
{
    public record User(int Id, string Login, string PasswordHash, string Role);
}
