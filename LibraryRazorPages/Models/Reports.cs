using LibraryRazorPages.DAL.Reports;

namespace LibraryRazorPages.Models.Reports
{
    public class BookHeading
    {
        [ReportColumn("ИД", Order = 1)]
        public int Id { get; set; }

        [ReportColumn("Книга", Order = 2)]
        public string? Name { get; set; }

        [ReportColumn("Заголовок", Order = 3)]
        public string? Heading { get; set; }
    }

    public class BookHeadingCount
    {
        [ReportColumn("ИД", Order = 1)]
        public int Id { get; set; }

        [ReportColumn("Книга", Order = 2)]
        public string? Name { get; set; }

        [ReportColumn("Заголовков", Order = 3)]
        public int HeadingCount { get; set; }
    }

    public class BookFirstHeading
    {
        [ReportColumn("ИД", Order = 1)]
        public int Id { get; set; }

        [ReportColumn("Книга", Order = 2)]
        public string? Name { get; set; }

        [ReportColumn("Первый заголовок", Order = 3)]
        public string? FirstHeading { get; set; }
    }
}