using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptINItemListModel
    {
        public string Company { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public long ItemID { get; set; }
        public string ItemDescription { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}