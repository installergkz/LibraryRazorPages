using LibraryRazorPages.DAL.Reports;
using System.Reflection;

namespace LibraryRazorPages.Extensions
{
    public static class ReportColumnExtensions
    {
        public static string GetHeader(this PropertyInfo prop) => prop.GetCustomAttribute<ReportColumnAttribute>()?.Header ?? prop.Name;

        public static int GetOrder(this PropertyInfo prop) => prop.GetCustomAttribute<ReportColumnAttribute>()?.Order ?? int.MaxValue;

        public static string? GetFormat(this PropertyInfo prop) => prop.GetCustomAttribute<ReportColumnAttribute>()?.Format;
    }
}