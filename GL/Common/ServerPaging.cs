using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GL.Common
{
    // Paging, sorting and search sent by serverPagedList() in js/Common/common.js.
    public class ServerPageRequest
    {
        public int PageDraw { get; set; }
        public int PageStart { get; set; }
        // 0 = request did not come from serverPagedList (return every row), -1 = "All"
        public int PageLength { get; set; }
        public string PageSearch { get; set; }
        public int? PageOrderColumn { get; set; }
        public string PageOrderDir { get; set; }
    }

    public static class ServerPaging
    {
        // columns[i] is the property shown in table column i (null = not searchable/sortable).
        // Returns only the requested page and reports the row counts in response headers.
        public static List<T> Apply<T>(IEnumerable<T> rows, ServerPageRequest request, HttpResponseBase response, params string[] columns)
        {
            var list = rows == null ? new List<T>() : rows.ToList();
            if (request == null || request.PageLength == 0)
                return list;

            var props = columns.Select(c => c == null ? null : typeof(T).GetProperty(c)).ToArray();
            int total = list.Count;

            if (!string.IsNullOrWhiteSpace(request.PageSearch))
            {
                string term = request.PageSearch.Trim();
                var searchProps = props.Where(p => p != null && Nullable.GetUnderlyingType(p.PropertyType) != typeof(bool) && p.PropertyType != typeof(bool)).ToList();
                list = list.Where(r => searchProps.Any(p => Format(p.GetValue(r)).IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            }
            int filtered = list.Count;

            int col = request.PageOrderColumn.GetValueOrDefault(-1);
            if (col >= 0 && col < props.Length && props[col] != null)
            {
                PropertyInfo p = props[col];
                list = string.Equals(request.PageOrderDir, "desc", StringComparison.OrdinalIgnoreCase)
                    ? list.OrderByDescending(r => p.GetValue(r)).ToList()
                    : list.OrderBy(r => p.GetValue(r)).ToList();
            }

            if (request.PageLength > 0)
                list = list.Skip(Math.Max(request.PageStart, 0)).Take(request.PageLength).ToList();

            response.AppendHeader("X-Total-Count", total.ToString());
            response.AppendHeader("X-Filtered-Count", filtered.ToString());
            return list;
        }

        private static string Format(object value)
        {
            if (value is DateTime)
                return ((DateTime)value).ToString("dd-MMM-yyyy");
            return Convert.ToString(value);
        }
    }
}
