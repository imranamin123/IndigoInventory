using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;
using GL.EF;
using GL.Models;
using GL.ReportsWebForms;
using System.Runtime.InteropServices.ComTypes;

namespace GL.DAL
{
    public class DALIngegration
    {
        private GLEntities db = new GLEntities();

        public void InsertBankTransactionVoucher(BKBankTran bkBankTrans, BKBank bkBank, string VTYPE, int BankTransTypeID)
        {
            var voucher = new GLVoucher();
            voucher.VoucherDate = bkBankTrans.TransDate;
            voucher.CreatedBy = bkBankTrans.CreatedBy;
            voucher.CreatedAt = bkBankTrans.CreatedAt;
            voucher.ModifiedBy = bkBankTrans.ModifiedBy;
            voucher.ModifiedAt = bkBankTrans.ModifiedAt;
            voucher.CompanyID = bkBankTrans.CompanyID;
            voucher.VoucherTypeID = db.VoucherTypes.Where(x => x.CompanyID == bkBankTrans.CompanyID && x.VTYPE == VTYPE).FirstOrDefault().VoucherTypeID; //voucherType.VoucherTypeID;

            var FiscalYearSetup = new DALCommon().GetFiscalYearSetup(bkBankTrans.CompanyID.Value, bkBankTrans.TransDate.Value.Year, bkBankTrans.TransDate.Value.Month);
            if (FiscalYearSetup != null)
            {
                voucher.FiscalYear = FiscalYearSetup.FiscalYear.ToString() + "-" + FiscalYearSetup.FiscalPeriod.ToString();
            }

            db.GLVouchers.AddOrUpdate(voucher);
            db.SaveChanges();

            voucher.VoucherNumber = VTYPE + "-" + voucher.GLVoucherID;
            db.GLVouchers.AddOrUpdate(voucher);
            db.SaveChanges();

            var bkBankTransDetails = db.BKBankTransDetails.Where(x => x.BankTransID == bkBankTrans.BankTransID && x.IsPost == 1 && x.BankTransTypeID == BankTransTypeID).ToList();// 1=CP, Cash payment
                                                                                                                                                                                  //var bkBankTransDetails = db.BKBankTransDetails.Where(x => x.BankTransID == bkBankTrans.BankTransID).ToList();// 1=CP, Cash payment

            if (bkBankTransDetails != null && bkBankTransDetails.Count > 0)
            {
                decimal? TotalDebit = 0;
                decimal? TotalCreadit = 0;
                foreach (var item in bkBankTransDetails)
                {
                    var GLAccountID = db.GLAccounts.Where(x => x.GLAccountNo.Trim().Replace("\r\n", "") == bkBank.GLAccountNo.Trim().Replace("\r\n", "") && x.CompanyID == item.CompanyID).FirstOrDefault().GLAccountID;

                    var voucherDetail = new GLVoucherDetail();
                    voucherDetail.GLVoucherID = voucher.GLVoucherID;
                    voucherDetail.GLAccountID = GLAccountID;
                    voucherDetail.GLNarration = item.DocumentRef;
                    voucherDetail.CreatedAt = item.CreatedAt;
                    voucherDetail.CreatedBy = item.CreatedBy;
                    voucherDetail.ModifiedAt = item.ModifiedAt;
                    voucherDetail.ModifiedBy = item.ModifiedBy;
                    voucherDetail.CompanyID = item.CompanyID;

                    if (item.Dr != null && item.Dr > 0)
                    {
                        voucherDetail.Debit = item.Dr;
                        voucherDetail.Credit = 0;
                        TotalDebit += item.Dr;
                    }
                    else if (item.Cr != null && item.Cr > 0)
                    {
                        voucherDetail.Credit = item.Cr;
                        voucherDetail.Debit = 0;
                        TotalCreadit += item.Cr;
                    }

                    new DALGLVoucher().GLVoucherDetailSave(voucherDetail);


                    int GLAccountID2 = db.GLAccounts.Where(x => x.A1 == "00" && x.A2 == "00" && x.A3 == "00" & x.A4 == "0000" && x.CompanyID == item.CompanyID).FirstOrDefault().GLAccountID;

                    var voucherDetail2 = new GLVoucherDetail();
                    voucherDetail2.GLVoucherID = voucher.GLVoucherID;
                    voucherDetail2.GLAccountID = GLAccountID2;
                    voucherDetail2.GLNarration = item.DocumentRef;
                    voucherDetail2.CreatedAt = item.CreatedAt;
                    voucherDetail2.CreatedBy = item.CreatedBy;
                    voucherDetail2.ModifiedAt = item.ModifiedAt;
                    voucherDetail2.ModifiedBy = item.ModifiedBy;
                    voucherDetail2.CompanyID = item.CompanyID;

                    if (item.Dr != null && item.Dr > 0)
                    {
                        voucherDetail2.Credit = item.Dr;
                        voucherDetail2.Debit = 0;
                    }
                    else if (item.Cr != null && item.Cr > 0)
                    {
                        voucherDetail2.Debit = item.Cr;
                        voucherDetail2.Credit = 0;
                    }

                    new DALGLVoucher().GLVoucherDetailSave(voucherDetail2);

                }

            }
        }

        public bool IntBankTransactionVoucher(int bkBankTransID, int loginCompanyID)
        {
            var bkBankTrans = db.BKBankTrans.Where(x => x.BankTransID == bkBankTransID && (x.IsPosted == null || x.IsPosted == false)).FirstOrDefault();
            var bkBankTransDetailsT = db.BKBankTransDetails.Where(x => x.BankTransID == bkBankTransID).ToList();

            if (bkBankTrans != null)
            {
                var bkBank = db.BKBanks.Where(x => x.BankID == bkBankTrans.BankID).FirstOrDefault();

                int OPCount = bkBankTransDetailsT.Where(x => x.BankTransTypeID == 5 && x.IsPost == 1).Count();
                int CPCount = bkBankTransDetailsT.Where(x => x.BankTransTypeID == 1 && x.IsPost == 1).Count();
                int CRCount = bkBankTransDetailsT.Where(x => x.BankTransTypeID == 2 && x.IsPost == 1).Count();
                int BPCount = bkBankTransDetailsT.Where(x => x.BankTransTypeID == 3 && x.IsPost == 1).Count();
                int BRCount = bkBankTransDetailsT.Where(x => x.BankTransTypeID == 4 && x.IsPost == 1).Count();

                if (CPCount > 0)
                {
                    InsertBankTransactionVoucher(bkBankTrans, bkBank, "AP-CP", 1);
                }

                if (CRCount > 0)
                {
                    InsertBankTransactionVoucher(bkBankTrans, bkBank, "BK-CR", 2);//
                }

                if (BPCount > 0)
                {
                    InsertBankTransactionVoucher(bkBankTrans, bkBank, "AP-BP", 3);
                }

                if (BRCount > 0)
                {
                    InsertBankTransactionVoucher(bkBankTrans, bkBank, "BK-BR", 4);
                }

                if (OPCount > 0)
                {
                    InsertBankTransactionVoucher(bkBankTrans, bkBank, "GL-JE", 5);
                }

            }

            bkBankTrans.IsPosted = true;
            db.BKBankTrans.AddOrUpdate(bkBankTrans);
            db.SaveChanges();


            return true;
        }

        public bool IntDVReceiptVoucher(int DVReceiptID, int loginCompanyID)
        {
            try
            {

                var dvReceipt = db.DVReceipts.Where(x => x.DVReceiptID == DVReceiptID).FirstOrDefault();
                var dvReceiptDetails = db.DVReceiptDetails.Where(x => x.DVReceiptID == DVReceiptID).ToList();


                var receiptCash = db.spIntDVReceiptCash(dvReceipt.DVReceiptID).FirstOrDefault();

                if (receiptCash != null)
                {
                    var voucher = new GLVoucher();
                    var voucherDetails = new List<GLVoucherDetail>();

                    voucher.VoucherDate = dvReceipt.DVReceiptDate;
                    voucher.CreatedAt = dvReceipt.CreatedAt;
                    voucher.CreatedBy = dvReceipt.CreatedBy;
                    voucher.ModifiedAt = dvReceipt.ModifiedAt;
                    voucher.ModifiedBy = dvReceipt.ModifiedBy;
                    voucher.CompanyID = dvReceipt.CompanyID;
                    voucher.VoucherTypeID = receiptCash.VoucherTypeID;

                    var FiscalYearSetup = new DALCommon().GetFiscalYearSetup(dvReceipt.CompanyID.Value, dvReceipt.DVReceiptDate.Value.Year, dvReceipt.DVReceiptDate.Value.Month);
                    if (FiscalYearSetup != null)
                    {
                        voucher.FiscalYear = FiscalYearSetup.FiscalYear.ToString() + "-" + FiscalYearSetup.FiscalPeriod.ToString();
                    }

                    db.GLVouchers.AddOrUpdate(voucher);
                    db.SaveChanges();

                    voucher.VoucherNumber = receiptCash.VTYPE + "-" + voucher.GLVoucherID;
                    db.GLVouchers.AddOrUpdate(voucher);
                    db.SaveChanges();

                    if (dvReceiptDetails != null && dvReceiptDetails.Count > 0)
                    {
                        foreach (var item in dvReceiptDetails)
                        {
                            var GLAccountID = db.GLAccounts.Where(x => x.A1 == receiptCash.A1 && x.A2 == receiptCash.A2 && x.A3 == receiptCash.A3 && x.A4 == receiptCash.A4 && x.CompanyID == dvReceipt.CompanyID).FirstOrDefault().GLAccountID;

                            var voucherDetail = new GLVoucherDetail();
                            voucherDetail.GLVoucherID = voucher.GLVoucherID;
                            voucherDetail.GLAccountID = GLAccountID;
                            voucherDetail.GLNarration = item.Narration;
                            voucherDetail.CreatedAt = item.CreatedAt;
                            voucherDetail.CreatedBy = item.CreatedBy;
                            voucherDetail.ModifiedAt = item.ModifiedAt;
                            voucherDetail.ModifiedBy = item.ModifiedBy;
                            voucherDetail.CompanyID = item.CompanyID;

                            if (item.Amount != null && item.Amount > 0)
                            {
                                voucherDetail.Debit = item.Amount;
                                voucherDetail.Credit = 0;
                            }
                            else
                            {
                                voucherDetail.Credit = Math.Abs(item.Amount.Value);
                                voucherDetail.Debit = 0;
                            }

                            new DALGLVoucher().GLVoucherDetailSave(voucherDetail);

                            var dvPaymentPlanType = db.DVPaymentPlanTypes.Where(x => x.PaymentPlanTypeID == item.PaymentPlanTypeID).FirstOrDefault();
                            if (dvPaymentPlanType != null)
                            {

                                var dvApplicationForm = db.DVApplicationForms.Where(x => x.ApplicationFormID == dvReceipt.ApplicationFormID).FirstOrDefault();
                                if (dvApplicationForm != null)
                                {

                                    string A3 = string.Empty;

                                    if (loginCompanyID == 3)
                                    {
                                        A3 = "31";
                                    }
                                    else
                                    {
                                        A3 = dvApplicationForm.ProjectID == 1 ? "40" : "30";
                                    }


                                    if (dvApplicationForm.A4 == null || dvApplicationForm.A4 == string.Empty)
                                    {
                                        dvApplicationForm.A4 = "0999";

                                    }


                                    var GLAccountID2 = db.GLAccounts.Where(x => x.A1 == dvPaymentPlanType.A1 && x.A2 == dvPaymentPlanType.A2 && x.A3 == A3 && x.A4 == dvApplicationForm.A4).FirstOrDefault().GLAccountID;

                                    var voucherDetail2 = new GLVoucherDetail();
                                    voucherDetail2.GLVoucherID = voucher.GLVoucherID;
                                    voucherDetail2.GLAccountID = GLAccountID2;
                                    voucherDetail2.GLNarration = item.Narration;
                                    voucherDetail2.CreatedAt = item.CreatedAt;
                                    voucherDetail2.CreatedBy = item.CreatedBy;
                                    voucherDetail2.ModifiedAt = item.ModifiedAt;
                                    voucherDetail2.ModifiedBy = item.ModifiedBy;
                                    voucherDetail2.CompanyID = item.CompanyID;

                                    if (item.Amount != null && item.Amount > 0)
                                    {
                                        voucherDetail2.Credit = item.Amount;
                                        voucherDetail2.Debit = null;
                                    }
                                    else
                                    {
                                        voucherDetail2.Debit = Math.Abs(item.Amount.Value);
                                        voucherDetail2.Credit = null;
                                    }
                                    new DALGLVoucher().GLVoucherDetailSave(voucherDetail2);


                                    dvReceipt.IsPosted = true;
                                    db.DVReceipts.AddOrUpdate(dvReceipt);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }

        public bool IntAPInvoice(Int64 APInvoiceID, int loginCompanyID)
        {
            try
            {

                var APInvoice = db.APInvoices.Where(x => x.APInvoiceID == APInvoiceID).FirstOrDefault();
                var APInvoiceDetail = db.APInvoiceDetails.Where(x => x.APInvoiceID == APInvoiceID).ToList();


                var GLControlAccountID = db.APGLControlAccts.Where(x => x.CompanyID == loginCompanyID).FirstOrDefault().GLAccountID;

                if (GLControlAccountID != null)
                {
                    var voucher = new GLVoucher();
                    var voucherDetails = new List<GLVoucherDetail>();

                    voucher.VoucherDate = APInvoice.APInvoiceDate;
                    voucher.CreatedAt = APInvoice.CreatedAt;
                    voucher.CreatedBy = APInvoice.CreatedBy;
                    voucher.ModifiedAt = APInvoice.ModifiedAt;
                    voucher.ModifiedBy = APInvoice.ModifiedBy;
                    voucher.CompanyID = APInvoice.CompanyID;
                    voucher.VoucherTypeID = db.VoucherTypes.Where(x => x.VTYPE == "AP-IN" && x.CompanyID == loginCompanyID).FirstOrDefault().VoucherTypeID;

                    var FiscalYearSetup = new DALCommon().GetFiscalYearSetup(APInvoice.CompanyID.Value, APInvoice.APInvoiceDate.Value.Year, APInvoice.APInvoiceDate.Value.Month);
                    if (FiscalYearSetup != null)
                    {
                        voucher.FiscalYear = FiscalYearSetup.FiscalYear.ToString() + "-" + FiscalYearSetup.FiscalPeriod.ToString();
                    }

                    db.GLVouchers.AddOrUpdate(voucher);
                    db.SaveChanges();

                    voucher.VoucherNumber = "AP-IN" + "-" + voucher.GLVoucherID;
                    db.GLVouchers.AddOrUpdate(voucher);
                    db.SaveChanges();

                    if (APInvoiceDetail != null && APInvoiceDetail.Count > 0)
                    {
                        decimal invoiceAmount = 0;

                        foreach (var item in APInvoiceDetail)
                        {
                            var voucherDetail = new GLVoucherDetail();
                            voucherDetail.GLVoucherID = voucher.GLVoucherID;
                            voucherDetail.GLAccountID = item.GLAccountID;
                            voucherDetail.GLNarration = db.APVendors.Find(APInvoice.APVendorID.Value).APVendorName + " Invoice# " + Convert.ToString(APInvoice.APInvoiceNo);
                            voucherDetail.CreatedAt = item.CreatedAt;
                            voucherDetail.CreatedBy = item.CreatedBy;
                            voucherDetail.ModifiedAt = item.ModifiedAt;
                            voucherDetail.ModifiedBy = item.ModifiedBy;
                            voucherDetail.CompanyID = item.CompanyID;

                            //if (item.Amount != null && item.Amount > 0)
                            //{
                                voucherDetail.Debit = item.Amount;
                                voucherDetail.Credit = 0;
                            //}
                            //else
                            //{
                            //    voucherDetail.Credit = Math.Abs(item.Amount.Value);
                            //    voucherDetail.Debit = 0;
                            //}
                            invoiceAmount = invoiceAmount + item.Amount.GetValueOrDefault(0);
                            new DALGLVoucher().GLVoucherDetailSave(voucherDetail);
                        }

                        // Control Account Voucher
                        var voucherDetail2 = new GLVoucherDetail();
                        voucherDetail2.GLVoucherID = voucher.GLVoucherID;
                        voucherDetail2.GLAccountID = GLControlAccountID;
                        voucherDetail2.GLNarration = db.APVendors.Find(APInvoice.APVendorID.Value).APVendorName + " Invoice# " + Convert.ToString(APInvoice.APInvoiceNo);
                        voucherDetail2.CreatedAt = APInvoice.CreatedAt;
                        voucherDetail2.CreatedBy = APInvoice.CreatedBy;
                        voucherDetail2.ModifiedAt = APInvoice.ModifiedAt;
                        voucherDetail2.ModifiedBy = APInvoice.ModifiedBy;
                        voucherDetail2.CompanyID = APInvoice.CompanyID;

                        //if (APInvoice.APInvoiceAmount != null && APInvoice.APInvoiceAmount > 0)
                        //{
                        //    voucherDetail2.Debit = APInvoice.APInvoiceAmount.Value;
                        //    voucherDetail2.Credit = 0;
                        //}
                        //else
                        //{
                            voucherDetail2.Credit = Math.Abs(invoiceAmount);
                            voucherDetail2.Debit = 0;
                        //}

                        new DALGLVoucher().GLVoucherDetailSave(voucherDetail2);

                        //

                        APInvoice.IsPosted = true;
                        db.APInvoices.AddOrUpdate(APInvoice);
                        db.SaveChanges();

                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }

    }
}