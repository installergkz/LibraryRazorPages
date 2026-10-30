using System.ComponentModel.DataAnnotations;

namespace LibraryRazorPages.DAL.Settings
{
    using System.ComponentModel.DataAnnotations;

    public class MongoDbSettings
    {
        public const string SectionName = "MongoDb";

        [Required(ErrorMessage = "MongoDb:ConnectionString is required.")]
        public string ConnectionString { get; set; } = string.Empty;

        [Required(ErrorMessage = "MongoDb:DatabaseName is required.")]
        public string DatabaseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "MongoDb:CollectionName is required.")]
        public string CollectionName { get; set; } = string.Empty;
    }
}
