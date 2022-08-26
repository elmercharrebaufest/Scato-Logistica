<%@ Page Language="C#" AutoEventWireup="true" Inherits="MvcReportViewer.MvcReportViewer, MvcReportViewer" EnableSessionState="false" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ OutputCache Location="None" %>
<% Response.Cache.SetCacheability(HttpCacheability.NoCache); %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="https://scato.molinosagro.com.ar/Scato.web/Scripts/jquery-1.11.0.min.js"></script>
    <script type="text/javascript" src="https://scato.molinosagro.com.ar/Scato.web/Scripts/jquery-migrate-1.2.1.min.js"></script>
</head>
<body>
    <form id="reportForm" runat="server">
        <div>
            <asp:ScriptManager runat="server" AsyncPostBackTimeout="600000"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer" runat="server" Width="100%" Height="100%" SizeToReportContent="True" AsyncRendering="True" ShowPrintButton="True" ProcessingMode="Remote"></rsweb:ReportViewer>
        </div>
    </form>
    <style>
        #ReportViewer_fixedTable {
            width: 100%;
        }
    </style>
    <script type="text/html" id="non-ie-print-button">
        <div class="" style="font-family: Verdana; font-size: 8pt; vertical-align: top; display: inline-block; width: 28px; margin-left: 6px;">
            <table style="display: inline;" cellspacing="0" cellpadding="0">
                <tbody>
                    <tr>
                        <td height="28">
                            <div>
                                <div id="mvcreportviewer-btn-print" style="border: 1px solid transparent; border-image: none; cursor: default; background-color: transparent;">
                                    <table title="Print">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <input
                                                        id="PrintButton"
                                                        title="Print"
                                                        style="width: 16px; height: 16px;"
                                                        type="image"
                                                        alt="Print"
                                                        runat="server"
                                                        src="~/Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=11.0.3442.2&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif" />
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
