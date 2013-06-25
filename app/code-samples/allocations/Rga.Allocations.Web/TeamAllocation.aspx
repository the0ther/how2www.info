<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ page language="C#" autoeventwireup="true" CodeFile="TeamAllocation.aspx.cs" inherits="Allocations.TeamAllocation" trace="false" enableeventvalidation="false" %>
<%@ Register TagPrefix="RGAWC" Namespace="RGA.UI.WebControls" Assembly="RGA.UI.WebControls" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="topHeader" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="Footer" %>
<%@ Register TagPrefix="RGAm" Namespace="Allocations" Assembly="topNav" %>
<%@ Register TagPrefix="rga" TagName="WeekPicker" Src="~/Controls/WeekPicker.ascx" %>
<%@ Register TagPrefix="rga" TagName="Alloc" Src="~/Controls/TeamAllocationWithNote.ascx" %>
<%@ Register TagPrefix="rga" TagName="AssignedJobsIcon" Src="~/Controls/AssignJobsIcon.ascx" %>
<html>
<head>
<title>Team Allocations</title>
<link rel="stylesheet" href="Css/Allocations.css" type="text/css" />
<link rel="stylesheet" href="Css/Common.css" type="text/css" />
<link rel="Stylesheet" href="Css/Popup.css" type="text/css" />
<link rel="stylesheet" href="css/blueprint/screen.css" type="text/css" media="screen, projection">
<link rel="stylesheet" href="css/blueprint/print.css" type="text/css" media="print">    
<!--[if IE]><link rel="stylesheet" href="css/blueprint/ie.css" type="text/css" media="screen, projection"><![endif]-->
<style type="text/css">
input.img { margin-left:-3px; position:absolute; }
</style>
<!--[if IE]>
<style type="text/css">
#notesPanel, #notesPanel div { width: 245px; }
div.employee-icon { border:medium none; float: right; position: relative; top: 4px; padding: 0px 10px 0 0; line-height: 20px; }
#notesPanel { height: 330px; }
#notesIFrame { height: 305px; }
</style>
<![endif]-->
</head>
<body>

	<rga:TopHeader ID="theader" pageID="319" ptitle="R/GA Management Tools: Time Tracking" runat="server" visible="" />
	<RGAm:Lev0Nav ID="aheader" runat="server" PageID="192" lev1NavID="levaNav" lev2NavID="levbNav" />
	<RGAm:Lev1Nav ID="levaNav" runat="server" />
	<div id='content' class="contentArea">
	<form id="form1" runat="server">
	
	
    <div id="allocation-controls">
		<div class="controls-section">
			<asp:DropDownList ID="dropClient" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropClient_SelectedIndexChanged"
						Width="150px"/>
			<div class="clear">&nbsp;</div>
			<asp:RadioButtonList ID="radClientOptions" runat="server" CellPadding="6" RepeatLayout="Table" RepeatDirection="Horizontal"
					OnSelectedIndexChanged="radClientOptions_SelectedIndexChanged" AutoPostBack="True"
					CssClass="tight small">
			<asp:ListItem Value="ALL">All</asp:ListItem>
			<asp:ListItem Value="ASSIGNED" Selected="True">Assigned</asp:ListItem>
			</asp:RadioButtonList>
		</div>
		<div class="controls-section border2">
			<p>
				<strong>Resources</strong>
			</p>
			<asp:RadioButtonList ID="radResourceOptions" runat="server" AutoPostBack="true" RepeatColumns="2"
				OnSelectedIndexChanged="radResourceOptions_SelectedIndexChanged" CssClass="tight">
			<asp:ListItem Value="ASSIGNED" Selected="True">Assigned</asp:ListItem>
			<asp:ListItem Value="ALLOCATED">Allocated</asp:ListItem>
			<asp:ListItem Value="UNALLOCATED">Unallocated</asp:ListItem>
			<asp:ListItem Value="OVERALLOCATED">Over allocated</asp:ListItem>
			<asp:ListItem Value="UNDERALLOCATED">Under allocated</asp:ListItem>
			<asp:ListItem Value="TBD">TBD</asp:ListItem>
			</asp:RadioButtonList>
			<span class="msg">for current week</span>
		</div>
		<div class="controls-section border2">
			<asp:DropDownList ID="dropDepartment" runat="server" AutoPostBack="true" 
					OnSelectedIndexChanged="dropDepartment_SelectedIndexChanged"/>
			<br />
			<asp:DropDownList ID="dropClientTeam" runat="server" OnSelectedIndexChanged="dropClientTeam_SelectedIndexChanged"
					AutoPostBack="True">
			</asp:DropDownList>
			<br />
			<asp:Literal ID="ManageLink" runat="server" />
		</div>
		<div class="controls-section border2">
			<asp:DropDownList ID="RegionsDrop" runat="server" OnSelectedIndexChanged="RegionsDrop_SelectedIndexChanged" 
				AutoPostBack="true"/>
		</div>
	</div>
		
	<div id="allocation-date-selection">
		<rga:WeekPicker ID="ctlWeekPicker" runat="server" ShowDoubleArrows="true" />
	</div>

<div id="team-allocations" class="container">
    <!-- TOP LEVEL ROW DARK BACKGROUND STARTS -->
    <div class="head rowmain span-24 last">
        <div class="span-9 title border first left">
            <img class="toggle-all" src="./Img/icn_open.gif" alt="toggle" /><span>Resources</span>
        </div>
        <div class="span-1 cent border">Dept</div>
        <div class="span-2 cent border" title="Budgeted by title / Used by title">Bdgt/Used</div>
        <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekOne" runat="server">&nbsp;</asp:Literal></strong></div>
        <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekTwo" runat="server">&nbsp;</asp:Literal></strong></div>
        <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekThree" runat="server">&nbsp;</asp:Literal></strong></div>
        <div class="span-3 cent border last"><strong><asp:Literal ID="ltrWeekFour" runat="server">&nbsp;</asp:Literal></strong></div>
    </div>
    <!-- TOP LEVEL ROW ENDS -->
    
    <!-- THIS GRID IS USERS WITH THEIR JOBS LISTED BENEATH -->
	<asp:Repeater ID="repResources" runat="server" EnableViewState="true" OnItemDataBound="repResources_ItemDataBound">
	<ItemTemplate>
        <!-- USER ROW STARTS -->
        <div class="rowmain deptrow span-24 last">
            <div class="span-9 left border first">
				<rga:AssignedJobsIcon ID="AssignedJobs" runat="server" ClientId2='<%# _ClientId %>' CurrentWeek='<%# ctlWeekPicker.CurrentWeek %>'
				        LoginUserid="<%# base.login_user_id %>" RealPerson='<%# Eval("RealPerson") %>' UserId='<%# Eval("UserId") %>' 
				        UserType="<%# _secType %>"/>
			    <img class="toggle-row" id="toggleImage_<%# Eval("UserId") %>" src="img/icn_open.gif" alt="toggle" />
				<a href='EmployeeAllocation.aspx?EmpId=<%#Eval("UserId") %>&Week=<%#_Weeks[0] %>'
					id="employee_<%# Eval("UserId") %>" 
					title="<%# Eval("FirstName") %><%# (Eval("LastName").ToString()!="&nbsp;") ? " " + Eval("LastName") : string.Empty%><%# (Eval("Title").ToString()!=string.Empty) ? " / " + Eval("Title") : string.Empty %>">
					<%# Eval("FirstName") %><%# (Eval("LastName").ToString()!="&nbsp;") ? " " + Eval("LastName") : string.Empty %> 
				</a>
                <%# Eval("Title").ToString() != string.Empty ? " / " : "" %>
				<span title="<%# Eval("FirstName") %> <%# Eval("LastName") %> / <%# Eval("Title") %>" class="toggle-row">
				<%# (Eval("FirstName").ToString().Length + Eval("LastName").ToString().Length + Eval( "Title" ).ToString().Length) < 40 ? Eval("Title") : Eval("Title").ToString().Substring(0,20) + "..." %></span>
            </div>
            <div class="span-1 cent border"><span title="<%# Eval( "Name" ).ToString() %>"><%# Eval("ShortName")%></span></div>
            <div class="span-2 border">&nbsp;</div>
            <div class="span-3 border cent ">
                <div class="span-1 cent">
			        <span title="Available / Allocated this client / Allocated all projects" id="weekavailable_<%#Eval( "Week1" )%>_<%#Eval( "UserId" )%>"  class="<%# Convert.ToInt32 (Eval( "Week1AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week1AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%#Eval( "Week1AvailableHours" )%></span>&nbsp;
			    </div>
			    <div class="span-1 cent">
			        (<span title="Available / Allocated this client / Allocated all projects" id="week_<%#Eval( "Week1" )%>_<%#Eval( "UserId" )%>"  class="<%# Convert.ToInt32 (Eval( "Week1AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week1AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursToClient" )))%></span>)
			    </div>
			    <div class="span-1 cent last">
			        &nbsp;<span title="Available / Allocated this client / Allocated all projects" id="weektotal_<%#Eval( "Week1" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week1AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week1AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" )))%></span>
			    </div>
            </div>
            <div class="span-3 border cent ">
                <div class="span-1 cent">
                    <span title="Available / Allocated this client / Allocated all projects" id="weekavailable_<%#Eval( "Week2" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week2AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week2AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%#Eval( "Week2AvailableHours" )%></span>
                </div>
                <div class="span-1 cent">
                    (<span title="Available / Allocated this client / Allocated all projects" id="week_<%#Eval( "Week2" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week2AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week2AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursToClient" )))%></span>)
                </div>
                <div class="span-1 cent last">
		                <span title="Available / Allocated this client / Allocated all projects" id="weektotal_<%#Eval( "Week2" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week2AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week2AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" )))%></span>
                </div>
            </div>
            <div class="span-3 border cent ">
                <div class="span-1 cent">
                    <span title="Available / Allocated this client / Allocated all projects" id="weekavailable_<%#Eval( "Week3" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week3AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week3AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%#Eval( "Week3AvailableHours" )%></span>
                </div>
                <div class="span-1 cent">
                    (<span title="Available / Allocated this client / Allocated all projects" id="week_<%#Eval( "Week3" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week3AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week3AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursToClient" )))%></span>)
                </div>
                <div class="span-1 cent last">
                    <span title="Available / Allocated this client / Allocated all projects" id="weektotal_<%#Eval( "Week3" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week3AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week3AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" )))%></span>
                </div>
            </div>
            <div class="span-3 border cent last ">
                <div class="span-1 cent">
                    <span title="Available / Allocated this client / Allocated all projects" id="weekavailable_<%#Eval( "Week4" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week4AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week4AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%#Eval( "Week4AvailableHours" )%></span>
                </div>
                <div class="span-1 cent">
                    (<span title="Available / Allocated this client / Allocated all projects" id="week_<%#Eval( "Week4" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week4AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week4AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursToClient" )))%></span>)
                </div>
                <div class="span-1 cent last">
                    <span title="Available / Allocated this client / Allocated all projects" id="weektotal_<%#Eval( "Week4" )%>_<%#Eval( "UserId" )%>" class="<%# Convert.ToInt32 (Eval( "Week4AvailableHours" )) > RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" ))) ? "under" : ( Convert.ToInt32 (Eval( "Week4AvailableHours" )) < RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" ))) ? "over" : "" ) %>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" )))%></span>		
                </div>
            </div>
		    <asp:Repeater ID="repJobs" runat="server" EnableViewState="true">
		    <ItemTemplate>
            <!-- JOBLIST FOR USER ABOVE STARTS -->
            <div class="rowalt span-24 last">
                <div class="left span-9 border first">
				    <a class="name" href="ProjectAllocation.aspx?ProjectId=<%#Eval("JobId") %>&Week=<%# _Weeks[0] %>" title="<%# Eval("JobCode") + " - " + HttpUtility.HtmlEncode(Eval( "JobName" ) + ((Eval("StartDate")==DBNull.Value) ? "" : " (" + Convert.ToDateTime(Eval("StartDate")).ToString("MM/dd/yyyy") + " - ") + ((Eval("EndDate")==DBNull.Value) ? "" : Convert.ToDateTime(Eval("EndDate")).ToString("MM/dd/yyyy")) + ")") %>">
				    <%# Eval( "JobName" ).ToString().Length <= 42 ? Eval( "JobName" ).ToString() : Eval( "JobName" ).ToString().Substring(0, 42) + "..."%>
				    </a>
                    <asp:Label ID="UnassignedStar" runat="server" CssClass="red" Visible='<%#( Eval("Assigned").ToString()=="1") ? true : false %>'>*</asp:Label>
                </div>
                <div class="span-1 border right">&nbsp;</div>
                <div class="span-2 border right"><span><%# Eval( "TitleHoursBudgeted" )%> / <%# Eval( "TitleHoursUsed") %></span></div>
                <div class="span-3 border cent" title="Available / Allocated this client/ Allocated all projects">
				    <rga:Alloc ID="Week1Alloc" runat="server" AllocationId='<%# Eval("Week1AllocationId") %>' 
					    Client='<%# _ClientId %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
					     JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week1AllocationMins") %>' Note='<%# Eval("Week1AllocNote") %>'
					      UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[0] %>' />
                </div>
                <div class="span-3 border cent">
			        <rga:Alloc ID="Week2Alloc" runat="server" AllocationId='<%# Eval("Week2AllocationId") %>' 
				        Client='<%# _ClientId %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
				         JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week2AllocationMins") %>' Note='<%# Eval("Week2AllocNote") %>'
				          UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[1] %>' />
                </div>
                <div class="span-3 border cent">
				    <rga:Alloc ID="Week3Alloc" runat="server" AllocationId='<%# Eval("Week3AllocationId") %>' 
					    Client='<%# _ClientId %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
					     JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week3AllocationMins") %>' Note='<%# Eval("Week3AllocNote") %>'
					      UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[2] %>' />                                        
                </div>
                <div class="span-3 border cent last">                                
				    <rga:Alloc ID="Week4Alloc" runat="server" AllocationId='<%# Eval("Week4AllocationId") %>' 
					    Client='<%# _ClientId %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
					     JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week4AllocationMins") %>' Note='<%# Eval("Week4AllocNote") %>'
					      UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[3] %>' />
                </div>
            </div>                                                                   
            <!-- JOBLIST ENDS -->    
		    </ItemTemplate>
		    </asp:Repeater>
		</div>
		<!-- USER ROW ENDS -->
	</ItemTemplate>
	</asp:Repeater>
		
</div>

	<div id="table-below">
		<div class="clear">&nbsp;</div> 
		<div id="table-guide">
		<img src="Img/icn_overAllocated.gif" width="9" height="9" alt="Over Allocated" title="Over Allocated" />Over Allocated
		<img src="Img/icn_underAllocated.gif" width="9" height="9" alt="Under Allocated" title="Under Allocated" class="icon-margin" />Under Allocated
		<span class="divider"></span>Available | <strong>Allocated this client</strong> | Total </div>
		<div class="clear">&nbsp;</div>
	</div>
	
	<div id="menu1" class="menu">
		<ul><li><a href="javascript:;" id="UserAddJobsLink" onclick="showpopup(this.id, <%= ctlWeekPicker.CurrentWeek %>, <% = base.login_user_id %>);">Add Jobs</a></li></ul>
	</div>

	<div id="popupanel" class="modDig" style="position: fixed; left: 200px; width: 365px; top: 120px; height: 395px; display: none; background-color: #f2f2f2;">
	    <div style="width: 365px; background-color: #f2f2f2;">
	        <span style="float: right">&nbsp;<a href="javascript:;" onclick="hidepopup();" class="tbox" style="width: 15px;"><img src="Img/icon_close.gif" alt="Close Window" width="12px" height="11px" style="padding: 9px 11px 0 0;"/></a></span>
	    </div>
	    <iframe id="popupiframe" frameborder="0" width="360px" height="370px" style="border: none 0px #f2f2f2">Loading...</iframe>
	</div>
	
	<div id="notesPanel" class="modDig" style="display: none;">
	    <div>
	        <span>&nbsp;<a href="javascript:;" onclick="hideNotesPopup();" class="tbox" style="width: 15px;"><img src="Img/icon_close.gif" alt="Close Window" width="12px" height="11px" style="padding: 9px 11px 0 0;"/></a></span>
	    </div>
	    <iframe name="notesIFrame" id="notesIFrame" scrolling="no" frameborder="0" style="border: none 0px #f2f2f2; background-color: #f2f2f2;">Loading...</iframe>
	</div>
	
	<asp:HiddenField ID="togglesRecord" runat="server" EnableViewState="true" />
	<asp:HiddenField ID="rebind" runat="server" EnableViewState="true" />
	</form>
	</div>
	<rga:Footer ID="tfooter" runat="server" />
	<RGAm:Lev2Nav ID="levbNav" runat="server" />
	
	<!--Popup code-->
	<div class="popup" style="padding-left:10px;" >
	    <div class="popup-content">
	        <img src="img/icon_close.gif" class="close-popup" alt="close" />
	    </div>
    	<iframe id="popupIframe" scrolling="no" frameborder="0" ></iframe>
	</div>
	
<script src="Js/jquery-1.2.6.min.js" type="text/javascript"></script>
<script src="Js/jquery.dimensions.min.js" type="text/javascript"></script>
<script src="Js/firebug.js" type="text/javascript"></script>
<script src="Js/shared.js" type="text/javascript"></script>
<script src="Js/popup.js" type="text/javascript"></script>
<script src="Js/Note.js" type="text/javascript"></script>	
<script src="Js/toggle.js" type="text/javascript"></script>
<script type="text/javascript">
function openManagePanel()
{
	var client = $('#dropClient option:selected').val();

	var p = new Popup({
		pWidth: 824, pHeight: 454, iWidth: 815, iHeight: 420, src: 'ManageTeams.aspx?ClientId=' + client 
	});

	p.open();
	return false;
}

$(document).ready(function(){
    var CurrentHours;
   
    $('input.img').click(function() {
        var parts = $(this).attr('id').split('_');
        if (parts[1]!=0)
            showNotesPopup(parts[1],parts[2]);
        return false;
    });
   
    $('input.txt').focus(function() {
        if (IsQuarterHourIncrement($(this)))
            CurrentHours = $(this).val();
    });
    
    $('input.txt').blur(function () {
        var newAllocId;
        var noteLen;
        var parts = $(this).attr('id').split('_');
        var weekNum = parts[1];
        var employeeId = parts[2];
        var jobId = parts[3];
        //console.log('weekNum: ' + weekNum + ' employeeId: ' + employeeId + ' jobId: ' + jobId);
        
        if (IsQuarterHourIncrement($(this)))
	    {
	        //console.log('in here');
            var num = $.trim($(this).val());
            //console.log('num is: ' + num);
		    var weekAvailableId = "weekavailable_" + weekNum + "_" + employeeId;
		    var weekId = "week_" + weekNum + "_" + employeeId;
		    var weekTotalId = "weektotal_" + weekNum + "_" + employeeId;         
		    var weekAvailableHours = 0;
		    var weekHours = 0;
		    var weekTotalHours = 0;
		    //console.log('num is: ' + num + ' and current is: ' + CurrentHours);
		    var HoursDifference = num - CurrentHours;    
		    //console.log('hrs diff: ' + HoursDifference);
		    if (HoursDifference != 0)
		    {
			    var weekAvailableObj = $('#' + weekAvailableId); 
			    var weekObj = $('#' + weekId); 
			    var weekTotalObj = $('#' + weekTotalId); 
			    weekAvailableHours = parseFloat($(weekAvailableObj).text())
			    weekHours = parseFloat($(weekObj).text()) + HoursDifference
			    weekTotalHours = parseFloat($(weekTotalObj).text()) + HoursDifference
			    $(weekObj).text(weekHours);
			    $(weekTotalObj).text(weekTotalHours);
			    if (weekAvailableHours > weekTotalHours)
			    {
				    $(weekAvailableObj).attr('class', "under");
				    $(weekTotalObj).attr('class',"under");
				    $(weekObj).attr('class',"under");
			    }
			    else if (weekAvailableHours < weekTotalHours)
			    {
				    $(weekAvailableObj).attr('class', "over");
				    $(weekTotalObj).attr('class',"over");
				    $(weekObj).attr('class',"over");
			    }
			    else
			    {
				    $(weekAvailableObj).attr('class', "");
				    $(weekTotalObj).attr('class',"");
				    $(weekObj).attr('class',"");
			    }
			    //console.log('about to do the allocation...');
			    //update DB
			    var textbox = $(this);
			    var url = "Services/UpdateAllocation.aspx?allocationid=-1&mins=" + num * 60 + "&jobid=" + jobId + "&employeeid=" + employeeId + "&week=" + weekNum;
			    $.get(url, function(data){
			        //console.log('made the ajax call setting up results..');
                    newAllocId = eval((data))[0].AllocId;
			        noteLen = eval((data))[0].NoteLength;
			        //console.log('new id is: ' + newAllocId + ' and noteLen is: ' + noteLen);
                    var imgBtn = $(textbox).parent().siblings('.img');
                    $(imgBtn).attr('id','allocNote_' + newAllocId + '_true');
                    //console.log('num is : ' + num);
			        if (parseFloat(num) > 0)
			        {
			            //console.log('did the allocation, going to setup the image button now...');
				        //try to change the id of this allocation's note-button, and change the button's visibility
				        $(imgBtn).unbind('click');
                        $(imgBtn).click(function() {
                            showNotesPopup(newAllocId, true); 
                            return false;
                        });
                        $(imgBtn).css('cursor','pointer');
				        if (noteLen <= 0)
					        $(imgBtn).attr('src','Img/icon_note_off.gif');
				        else
					        $(imgBtn).attr('src','Img/icon_note_on.gif');
			        }
			        else
			        {
				        //disable the notes button since this is an allocation with 0 hours
				        $(imgBtn).attr('src','Img/c.gif');
                        $(imgBtn).css('cursor','default');
                        $(imgBtn).unbind('click');
			        }
			    });
		    }
	    }
    });
    
});
</script>
</body>
</html>
