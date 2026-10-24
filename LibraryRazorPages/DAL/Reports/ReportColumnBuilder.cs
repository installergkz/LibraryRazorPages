using System.Reflection;

namespace LibraryRazorPages.DAL.Reports
{
    public static class ReportColumnBuilder
    {
        public static IList<ReportColumn> Build(Type modelType)
        {
            var result = new List<ReportColumn>();

            foreach (var prop in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = prop.GetCustomAttribute<ReportColumnAttribute>();
                if (attr == null)
                    continue;

                result.Add(new ReportColumn
                {
                    Field = prop.Name,
                    Header = attr.Header,
                    Order = attr.Order,
                    Format = attr.Format
                });
            }

            return result.OrderBy(c => c.Order).ThenBy(c => c.Header).ToList();
        }
    }

    public class ReportColumn
    {
        public string Field { get; set; }
        public string Header { get; set; }
        public int Order { get; set; }
        public string Format { get; set; }
    }
}