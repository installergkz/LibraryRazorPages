using System.Data;
using System.Data.Common;

namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection();
    }
}
