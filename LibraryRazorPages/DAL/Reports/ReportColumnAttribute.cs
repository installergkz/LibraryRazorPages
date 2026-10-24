namespace LibraryRazorPages.DAL.Reports
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ReportColumnAttribute : Attribute
    {
        public string Header { get; }
        public int Order { get; set; } = 0;
        public string Format { get; set; }

        public ReportColumnAttribute(string header)
        {
            Header = header;
        }
    }
}