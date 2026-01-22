using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class SearchModel
    {
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public short? MonthID { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public Int64? StudentID { get; set; }
        public Int64? ID { get; set; }
        public string Name { get; set; }
        public string username { get; set; }
        public string Remarks { get; set; }
        public string Address { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsLoading { get; set; }
        public int? FeeStatus { get; set; }
        public Int64? FeeChallanID { get; set; }
        public Int64? FeeReceiptID { get; set; }
        public Nullable<short> Day { get; set; }
        public Nullable<int> UserID { get; set; }
        public Int16? StatusTypeID { get; set; }
        public Int16? StaffTypeID { get; set; }
        public Int16? DesignationID { get; set; }
        public int? ExpenseHeadID { get; set; }
        public Int16? RoleID { get; set; }
        public int? VoucherTypeID { get; set; }
        public int? ApplicationFormID { get; set; }
        public int? ProjectID { get; set; }
        public int? UnitID { get; set; }

        public bool IsAdmin { get; set; }
        public int MyProperty { get; set; }
    }
}