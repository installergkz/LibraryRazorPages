using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LibraryRazorPages.DAL.Helpers
{
    public static class HtmlToXmlConverter
    {
        private static readonly string[] VoidTags =
        {
            "area", "base", "br", "col", "embed", "hr", "img",
            "input", "link", "meta", "param", "source", "track", "wbr"
        };

        // HTML-мнемоники → числовые XML-коды.
        // Числовые коды валидны в XML без объявления DTD.
        private static readonly Dictionary<string, string> EntityMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["&nbsp;"] = "&#160;",
            ["&iexcl;"] = "&#161;",
            ["&cent;"] = "&#162;",
            ["&pound;"] = "&#163;",
            ["&curren;"] = "&#164;",
            ["&yen;"] = "&#165;",
            ["&brvbar;"] = "&#166;",
            ["&sect;"] = "&#167;",
            ["&uml;"] = "&#168;",
            ["&copy;"] = "&#169;",
            ["&ordf;"] = "&#170;",
            ["&laquo;"] = "&#171;",
            ["&not;"] = "&#172;",
            ["&shy;"] = "&#173;",
            ["&reg;"] = "&#174;",
            ["&macr;"] = "&#175;",
            ["&deg;"] = "&#176;",
            ["&plusmn;"] = "&#177;",
            ["&sup2;"] = "&#178;",
            ["&sup3;"] = "&#179;",
            ["&acute;"] = "&#180;",
            ["&micro;"] = "&#181;",
            ["&para;"] = "&#182;",
            ["&middot;"] = "&#183;",
            ["&cedil;"] = "&#184;",
            ["&sup1;"] = "&#185;",
            ["&ordm;"] = "&#186;",
            ["&raquo;"] = "&#187;",
            ["&frac14;"] = "&#188;",
            ["&frac12;"] = "&#189;",
            ["&frac34;"] = "&#190;",
            ["&iquest;"] = "&#191;",
            ["&times;"] = "&#215;",
            ["&divide;"] = "&#247;",
            ["&ndash;"] = "&#8211;",
            ["&mdash;"] = "&#8212;",
            ["&lsquo;"] = "&#8216;",
            ["&rsquo;"] = "&#8217;",
            ["&sbquo;"] = "&#8218;",
            ["&ldquo;"] = "&#8220;",
            ["&rdquo;"] = "&#8221;",
            ["&bdquo;"] = "&#8222;",
            ["&dagger;"] = "&#8224;",
            ["&Dagger;"] = "&#8225;",
            ["&bull;"] = "&#8226;",
            ["&hellip;"] = "&#8230;",
            ["&permil;"] = "&#8240;",
            ["&prime;"] = "&#8242;",
            ["&Prime;"] = "&#8243;",
            ["&lsaquo;"] = "&#8249;",
            ["&rsaquo;"] = "&#8250;",
            ["&oline;"] = "&#8254;",
            ["&frasl;"] = "&#8260;",
            ["&euro;"] = "&#8364;",
            ["&trade;"] = "&#8482;",
            ["&alpha;"] = "&#945;",
            ["&beta;"] = "&#946;",
            ["&gamma;"] = "&#947;",
            ["&delta;"] = "&#948;",
            ["&pi;"] = "&#960;",
            ["&sigma;"] = "&#963;",
            ["&omega;"] = "&#969;",
        };

        public static string? HtmlToWellFormedXml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return null;

            // 1. Парсим HTML5 через AngleSharp — получаем нормализованный DOM
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(html);
            var inner = doc.Body?.InnerHtml ?? html;

            // 2. Закрываем void-теги: <br> → <br/>, <img src="..."> → <img src="..."/>
            inner = CloseVoidTags(inner);

            // 3. Маппим HTML-мнемоники в числовые коды
            inner = ReplaceEntities(inner);

            // 4. Экранируем оставшиеся одиночные & (кроме уже валидных сущностей)
            inner = EscapeAmpersands(inner);

            return $"<content>{inner}</content>";
        }

        public static string? XmlToHtml(string? xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return null;

            try
            {
                var doc = XDocument.Parse(xml);
                return string.Concat(doc.Root.Nodes().Select(n => n.ToString()));
            }
            catch
            {
                return xml;
            }
        }

        private static string CloseVoidTags(string html)
        {
            foreach (var tag in VoidTags)
            {
                html = Regex.Replace(
                    html,
                    $@"<({tag})(\s[^>]*?)?\s*/?>",
                    m => $"<{m.Groups[1].Value}{m.Groups[2].Value}/>",
                    RegexOptions.IgnoreCase);
            }
            return html;
        }

        private static string ReplaceEntities(string html)
        {
            return Regex.Replace(html, @"&[a-zA-Z][a-zA-Z0-9]*;", m =>
                EntityMap.TryGetValue(m.Value, out var code) ? code : m.Value);
        }

        private static string EscapeAmpersands(string html)
        {
            // Экранируем & которые не являются началом числового кода или уже валидной XML-сущности
            return Regex.Replace(html, @"&(?!#\d+;|#x[0-9a-fA-F]+;|amp;|lt;|gt;|quot;|apos;)", "&amp;");
        }
    }
}