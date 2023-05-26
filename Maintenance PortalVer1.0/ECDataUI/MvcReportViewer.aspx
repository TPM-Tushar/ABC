<%@ Page Language="C#" AutoEventWireup="true" Inherits="MvcReportViewer.MvcReportViewer, MvcReportViewer" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%--<script  runat="server">
     try {
         Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
             document.getElementById("ReportViewer_fixedTable").style.tableLayout = '';
         });
     } catch (e) {
         //ignore me alert("error");
     }
     
    
    </script>--%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="//code.jquery.com/jquery-1.11.0.min.js"></script>
    <script type="text/javascript" src="//code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            $('#ParametersRowReportViewer').hide();
            $('#ReportViewer_ToggleParam_img').hide();

            //Below code added by rohit khatale on 09/05/2016 disaply report proper format in IE 10
            try {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    document.getElementById("ReportViewer_fixedTable").style.tableLayout = '';
                });
            } catch (e) {
                //ignore me alert("error");
            }
        });
</script>
   
</head>
<body>
    <form id="reportForm" runat="server">
    <div>
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer" ClientIDMode="Predictable" runat="server" ></rsweb:ReportViewer>
    </div>
    </form>

    <script type="text/html" id="non-ie-print-button">
        <table style="width: 6px; display: inline;" cellspacing="0" cellpadding="0" toolbarspacer="true">
            <tbody>
                <tr>
                    <td></td>
                </tr>
            </tbody>
        </table>
        <div class="" style="font-family: Verdana; font-size: 8pt; vertical-align: top; display: inline;">
            <table style="display: inline;" cellspacing="0" cellpadding="0">
                <tbody>
                    <tr>
                        <td height="28">
                            <div>
                                <div style="border: 1px solid transparent; border-image: none; cursor: default; background-color: transparent;">
                                    <table title="Print">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <input 
                                                        id="print-button"
                                                        title="Print" 
                                                        style="width: 16px; height: 16px;" 
                                                        type="image" 
                                                        alt="Print" 
                                                        src="/Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=11.0.3442.2&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif">
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </script>
</body>
</html>
