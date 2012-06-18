<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserJobs.aspx.cs" Inherits="Allocations.UserJobs" Trace="false"%>
<html>
<head runat="server">
    <title>User Jobs</title>
    <link href="Css/Allocations.css" type="text/css" rel="stylesheet"/>
    <link href="Css/Common.css" type="text/css" rel="stylesheet"/>
    <style type="text/css">
    #UserJobs_BoundingBox { margin: 10px }
    #UserJobs_BoundingBox p {margin: 10px 0 10px 0; }
    #UserJobs_BoundingBox h2 { font-size: 10pt; }
    #UserJobs_BoundingBox table, #UserJobs_BoundingBox td {background-color: #f2f2f2; }
    #JobList { width: 318px; border: solid 1px black; border-collapse: collapse;  margin: 0 0 0 1px;}
    td { border: solid 1px black; }
    th { height: 22px; }
    td.indent {text-indent: 12px; width: 293px; }
    th.indent { text-indent: 10px; }
    #JobList .header { background-color: #808080; color: #ffffff; height: 29px; }
    #ClearChecks { padding-bottom: 8px;}
    th { vertical-align: middle; font-size: 9pt; text-align: left; }
	.popup2 {
		position: absolute;
		top: 50px;
		border: 3px solid #ccc;
		background-color: #F2F2F2;
	}
	.popup-content2 {
		position: relative;
		margin: 10px;
		background-color: #F2F2F2;
	}
	.popup2 .close-popup {
		width: 12px; height: 11px; position: absolute;
		right: 0; top: 0; cursor: pointer;
	}
	td.first { text-align: center; width: 18px; }
	tr.header { background-color: #808080; color: #ffffff; padding-bottom: 3px; border: solid 1px black; }
	tr.header th { border: solid 0px #000000; }
	a.ctrl { font-weight: normal; color: #ff0000; }
    </style>
</head>
<body class="popup" id="UserJobs">
    <form id="form1" runat="server">
    <div id="UserJobs_BoundingBox">
<h2>Assign Jobs</h2>
<p>Select the jobs you want to assign <asp:Label ID="EmployeeName" runat="server" /> to.</p>
<!--
<div id="ClearChecks">
            <span id="ClearAll" class="red">Clear All</span>
            |
            <span id="SelectAll" class="red">Select All</span>
</div>
-->
<table cellpadding="0">

</table>
<asp:Panel ID="JobListScrollbox" runat="server" ScrollBars="auto" Height="200px">
    <asp:Repeater ID="repJobList" runat="server">
        <HeaderTemplate>
            <table id="JobList" cellpadding="3" cellspacing="0">
                <tr class="header" align="left">
                    <th style="width: 18px;">&nbsp;</th>
                    <th class="indent">Job Name</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="first">
                    <a href="javascript:;" jobid="<%#Eval("JobId") %>" title="add job" class="ctrl">Add</a>
                </td>
                <td class="indent" style="width: 293px;">
                    <%# Eval ("Name") %>
                </td>
            </tr>
        
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Panel>
<asp:HiddenField ID="ToMove" runat="server" />
<asp:HiddenField ID="ToDelete" runat="server" />
<asp:HiddenField ID="ToAssign" runat="server" />
<asp:HiddenField ID="ToUnassign" runat="server" />
<script src="Js/jquery-1.2.6.min.js" language="javascript" type="text/javascript"></script>
<script src="Js/jquery.query-1.2.3.js" language="javascript" type="text/javascript"></script>
<script src="Js/popup.js" language="javascript" type="text/javascript"></script>
<script type="text/javascript">
    $('a.ctrl').click(function() {
        var userId = $.query.get('UserId');
        assign($(this), $(this).attr('jobid'), userId);
        $(this).parent().parent().hide();
    });

    function assign(img, jobid, userid)
    {
	    $.get("Services/UpdateAssignment.aspx?UserId=" + userid + "&JobId=" + jobid + "&action=assign", function(data){});
    }
</script>
<script language="javascript" type="text/javascript">
    <asp:Literal ID="CloseScript" runat="server" />
    <asp:Literal ID="ResolveScript" runat="server" />
</script>
    </div>
    </form>
    <script src="Js/firebug.js" type="text/javascript"></script>
</body>
</html>
