<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportForm.aspx.cs" Inherits="GL.ReportsWebForms.ReportForm" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        @media print {
            /* Ensure the Crystal Reports viewer is displayed */
            #crystalReportViewer {
                display: block;
                width: 100%;
                height: auto;
                margin: 0;
                padding: 0;
            }
        }

        body * {
            visibility: hidden;
        }

        #viewer, #viewer * {
            visibility: visible;
        }

        @page {
            margin: 1cm;
        }

        .page-break {
            page-break-after: always;
        }

    </style>
    <script src="https://mozilla.github.io/pdf.js/build/pdf.js"></script>
</head>

<body>
    <%--<button onclick="window.print()" class="btn btn-success">Print Report</button>--%>
    <form id="form1" runat="server">
        <%--<button onclick="window.print()">Print Report</button>--%>
        
        <div id="viewer">
            <%--<CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true"  />--%>
            <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" />
        </div>
    </form>
</body>

<script>
    function printDiv(divName) {
        var printContents = document.getElementById(divName).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;

        window.print();

        //document.body.innerHTML = originalContents;
    }
</script>
</html>
