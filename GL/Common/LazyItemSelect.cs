using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GL.Common
{
    // Edit pages used to render the full item list (thousands of options) in every detail row,
    // which made documents with many rows tens of MB. Saved rows now render only their selected
    // item; common.js copies the full list in the first time a row's dropdown is opened.
    public static class LazyItemSelect
    {
        // Value -> text of every item option, built once per page from the ViewBag item list.
        public static Dictionary<string, string> Index(IEnumerable items, string valueField = "ItemID", string textField = "Description")
        {
            var index = new Dictionary<string, string>();
            if (items == null)
                return index;
            foreach (var option in new SelectList(items, valueField, textField))
            {
                if (option.Value != null && !index.ContainsKey(option.Value))
                    index[option.Value] = option.Text;
            }
            return index;
        }

        // An item <select id="ItemID"> holding "-- Select --" plus the row's item (when it is in the list).
        public static MvcHtmlString LazyItemDropDown(this HtmlHelper html, Dictionary<string, string> index, object selectedValue, string cssClass)
        {
            var select = new TagBuilder("select");
            select.MergeAttribute("id", "ItemID");
            select.MergeAttribute("name", "ItemID");
            select.AddCssClass("lazy-items");
            select.AddCssClass(cssClass);

            var options = new StringBuilder("<option value=\"\">-- Select --</option>");
            string value = Convert.ToString(selectedValue);
            string text;
            if (!string.IsNullOrEmpty(value) && index.TryGetValue(value, out text))
            {
                options.Append("<option selected=\"selected\" value=\"").Append(HttpUtility.HtmlAttributeEncode(value)).Append("\">")
                       .Append(HttpUtility.HtmlEncode(text)).Append("</option>");
            }
            select.InnerHtml = options.ToString();
            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }

        // A hidden full copy of the item list, for pages without a .trRowTemplate row to copy from.
        // It has the item-list class so project changes that rewrite .item-list keep it current.
        public static MvcHtmlString LazyItemSource(this HtmlHelper html, IEnumerable items)
        {
            return html.DropDownList("lazyItemSource", new SelectList(items, "ItemID", "Description"), "-- Select --",
                new { id = "lazyItemSource", @class = "item-list", style = "display:none" });
        }
    }
}
