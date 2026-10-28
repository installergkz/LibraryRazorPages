namespace LibraryRazorPages.DAL.SQL
{
    public static class Queries
    {
        public static class StoredProcedures
        {
            public const string GetBooks = "GetBooks";
            public const string CreateBook = "CreateBook";
            public const string UpdateBook = "UpdateBook";
            public const string DeleteBook = "DeleteBook";
            public const string GetPublicationHouses = "GetPublicationHouses";
            public const string GetBookHeadings = "GetBookHeadings";
            public const string GetHeadingCounts = "GetHeadingCounts";
            public const string GetFirstHeadings = "GetFirstHeadings";
            public const string SearchBooksByHeading = "SearchBooksByHeading";
            public const string CreateUser = "CreateUser";
            public const string GetUserByName = "GetUserByName";
            public const string GetUsers = "GetUsers";
            public const string CheckUserExists = "CheckUserExists";
            public const string UpdateUserRole = "UpdateUserRole";
        }
        
    }
}
