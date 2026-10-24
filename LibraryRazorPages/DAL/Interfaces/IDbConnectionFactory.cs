using System.Data;

namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
