using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class spRptVendorListModel
    {
        public string Company { get; set; }
        public int APVendorID { get; set; }
        public string APVendorName { get; set; }
        public string ContactPerson { get; set; }
        public string BankDetails { get; set; }
        public string APVendorCategoryName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}