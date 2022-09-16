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
	<script type="text/javascript" src="https://scato.molinosagro.com.ar/Scato.web/Scripts/bootstrap-datetimepicker.js"></script>
	<link type="text/css" href="https://scato.molinosagro.com.ar/Scato.web/Content/bootstrap-datetimepicker.css" rel="stylesheet"/>
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

<style>
.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  z-index: 1000;
  display: none;
  float: left;
  min-width: 160px;
  padding: 5px 0;
  margin: 2px 0 0;
  list-style: none;
  background-color: #ffffff;
  border: 1px solid #ccc;
  border: 1px solid rgba(0, 0, 0, 0.2);
  *border-right-width: 2px;
  *border-bottom-width: 2px;
 /* -webkit-border-radius: 6px;
     -moz-border-radius: 6px;
          border-radius: 6px;*/
  -webkit-box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
     -moz-box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
          box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
  -webkit-background-clip: padding-box;
     -moz-background-clip: padding;
          background-clip: padding-box;
}

[class^="icon-"],
[class*=" icon-"] {
  display: inline-block;
  width: 14px;
  height: 14px;
  margin-top: 1px;
  *margin-right: .3em;
  line-height: 14px;
  vertical-align: text-top;
  background-image: url("content/images/glyphicons-halflings.png");
  background-position: 14px 14px;
  background-repeat: no-repeat;
}

body {
    margin: 0;
    font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
    font-size: 14px;
    line-height: 20px;
    color: #333333;
}

.table-condensed td {
  padding: 4px 5px;
}

.icon-arrow-left {
  background-position: -240px -96px;
}

.icon-arrow-right {
  background-position: -264px -96px;
}
</style>


