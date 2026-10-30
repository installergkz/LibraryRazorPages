using Dapper.FluentMap;

namespace LibraryRazorPages.DAL.Mapping
{
    public static class FluentMappingInitializer
    {
        public static void Init() 
        {
            FluentMapper.Initialize(config =>
            {
                //config.AddMap(new BookMap());
            });
        }
    }
}
