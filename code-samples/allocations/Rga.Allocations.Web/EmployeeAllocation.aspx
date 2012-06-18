<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EmployeeAllocation.aspx.cs" Trace="false" Inherits="Allocations.EmployeeAllocation" %>
<%@ Register TagPrefix="RGAWC" Namespace="RGA.UI.WebControls" Assembly="RGA.UI.WebControls" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="topHeader" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="Footer" %>
<%@ Register TagPrefix="RGAm" Namespace="Allocations" Assembly="topNav" %>
<%@ Register TagPrefix="rga" TagName="WeekPicker" Src="~/Controls/WeekPicker.ascx" %>
<%@ Register TagPrefix="rga" TagName="Alloc" Src="~/Controls/EmpAllocationWithNote.ascx" %>
<html>
<head runat="server">
    <title>Employee Allocations</title>
    <link rel="stylesheet" href="Css/Allocations.css" type="text/css" />
    <link rel="stylesheet" href="Css/Common.css" type="text/css" />
    <link rel="stylesheet" href="css/blueprint/screen.css" type="text/css" media="screen, projection">
    <link rel="stylesheet" href="css/blueprint/print.css" type="text/css" media="print">    
    <!--[if IE]><link rel="stylesheet" href="css/blueprint/ie.css" type="text/css" media="screen, projection"><![endif]-->
    <style type="text/css">
    input.txt + input.img { float:right; margin-left:-3px; position:relative; top: -19px; }
    input.img { float:right; margin-left:-3px; position:relative; top: -23px; }
    </style>
    <!--[if IE]>
    <style type="text/css">
    #notesPanel, #notesPanel div { width: 245px; }
    #notesPanel { height: 330px; }
    #notesIFrame { height: 305px; }
    input.txt + input.img { float:right; margin-left:-3px; position:relative; top: -24px; }
    input.img { float:right; margin-left:-3px; position:relative; top: -23px; }
    input.txt { margin-bottom: 3px; }
    </style>
    <![endif]-->
</head>
<body>
<rga:TopHeader ID="theader" pageID="376" ptitle="R/GA Management Tools: Time Tracking" runat="server" />
<RGAm:Lev0Nav ID="aheader" runat="server" PageID="192" lev1NavID="levaNav" lev2NavID="levbNav" />
<RGAm:Lev1Nav ID="levaNav" runat="server" />
<div id='content' class="contentArea">
	<form id="form1" runat="server">
    <asp:HiddenField ID="togglesRecord" runat="server" EnableViewState="true" />
    
	<div id="allocation-controls">
		<div class="controls-section">
			<asp:DropDownList ID="DeptsDrop" runat="server" 
				OnSelectedIndexChanged="DeptsDrop_SelectedIndexChanged" AutoPostBack="true" 
				Width="250px"  /><br />
			<asp:DropDownList ID="EmpsDrop" runat="server" 
				OnSelectedIndexChanged="EmpsDrop_SelectedIndexChanged" AutoPostBack="true" 
				Width="250px" OnDataBound="EmpsDrop_DataBound" /><br />
			<asp:Label ID="Title" runat="server" Text="Current Title: " CssClass="msg" />
		</div>
	</div>

	<div class="clear"></div>

	<div id="allocation-date-selection">
		<rga:WeekPicker ID="ctlWeekPicker" runat="server" ShowDoubleArrows="true" />
	</div>

	<div id="emp-allocs" class="container">
	
        <!-- TOP LEVEL ROW DARK BACKGROUND STARTS -->
        <div class="head rowmain span-24 last">
            <div class="span-9 title border first left">
                <img class="toggle-all" src="./Img/icn_open.gif" alt="toggle" /><span>Client/Projects</span>
            </div>
            <div class="span-3 cent border" title="Budgeted by title / Used by title">Bdgt/Used</div>
            <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekOne" runat="server">&nbsp;</asp:Literal></strong></div>
            <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekTwo" runat="server">&nbsp;</asp:Literal></strong></div>
            <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekThree" runat="server">&nbsp;</asp:Literal></strong></div>
            <div class="span-3 cent border last"><strong><asp:Literal ID="ltrWeekFour" runat="server">&nbsp;</asp:Literal></strong></div>
        </div>
        <!-- TOP LEVEL ROW ENDS -->	   
        
        <!-- THIS GRID IS CLIENTS WITH A LIST OF JOBS BENEATH -->
		<asp:Repeater ID="repClients" runat="server" EnableViewState="true" OnItemDataBound="repClients_ItemDataBound">
			<HeaderTemplate>
			<div class="rowmain" style="background-color: #ffffff;">
				<div class="span-9 title border first left">&nbsp;</div>
				<div class="span-3 cent border">&nbsp;</div>
				<div class="span-3 cent border"><asp:Label ID="ltrWeek1Avail" runat="server" ToolTip="Available / Allocated" /></div>
				<div class="span-3 cent border"><asp:Label ID="ltrWeek2Avail" runat="server" ToolTip="Available / Allocated" /></div>
				<div class="span-3 cent border"><asp:Label ID="ltrWeek3Avail" runat="server" ToolTip="Available / Allocated" /></div>
				<div class="span-3 cent border last"><asp:Label ID="ltrWeek4Avail" runat="server" ToolTip="Available / Allocated" /></div>
			</div>
			</HeaderTemplate>
			<ItemTemplate>
            <!-- CLIENT ROW STARTS -->
            <div class="rowmain deptrow span-24 last">
                <div class="span-9 left border first">
                    <img id="toggleImage_<%# Eval("ClientId") %>" src="img/icn_open.gif" class="toggle-row" alt="toggle" />
                    <a class="toggle-row" href="javascript:;" id="employee_<%# Eval("ClientId") %>" title="<%# Eval("Name") %>">
                        <%# Eval("Name") %>
                    </a>
                </div>
                <div class="span-3 border">&nbsp;</div>
                <div class="span-3 border cent">&nbsp;</div>
                <div class="span-3 border cent">&nbsp;</div>
                <div class="span-3 border cent">&nbsp;</div>
                <div class="span-3 border cent last">&nbsp;</div>
			    <asp:Repeater ID="repJobs" runat="server" EnableViewState="true">
				    <ItemTemplate>					
                    <!-- JOBLIST FOR CLIENT ABOVE STARTS -->
                    <div class="rowalt span-24 last">
                        <div class="left span-9 border first">
						    <a class="name" href="ProjectAllocation.aspx?ProjectId=<%# Eval("JobId") %>&Week=<%# _Weeks[0] %>" 
							    title="<%# Eval("JobCode") + " - " + Eval( "JobName" ) + ((Eval("StartDate")==DBNull.Value) ? "" : " (" + Convert.ToDateTime(Eval("StartDate")).ToString("MM/dd/yyyy") + " - ") + ((Eval("EndDate")==DBNull.Value) ? "" : Convert.ToDateTime(Eval("EndDate")).ToString("MM/dd/yyyy")) + ")" %>">
							    <%# Eval("JobName").ToString().Length > 44 ? Eval("JobName").ToString().Substring(0,43) + "..." : Eval("JobName") %></a>                            
                                <asp:Label ID="UnassignedStar" runat="server" CssClass="red" Visible='<%#( Eval("Assigned").ToString()=="1") ? true : false %>'>*</asp:Label>
                        </div>
                        <div class="span-3 border right"><span><%#Eval("TitleHoursBudgeted") %>/<%# Eval("TitleHoursUsed") %></span></div>
                        <div class="span-3 border cent">
						    <rga:Alloc ID="Week1Alloc" runat="server" 
							    AllocationId='<%# Eval("Week1AllocId") %>' 
							    JobId='<%# Eval("JobId") %>' Minutes='<%# Eval("Week1AllocMins") %>' 
							    Note='<%# Eval("Week1AllocNote") %>'
							    UserId="<%# _EmpId %>" UserType="<%# _UserType %>" WeekNumber="<%# _Weeks[0] %>"
							    Start='<%# Eval("StartDate") %>' End='<%# Eval("EndDate") %>' EmpNoteClientId='<%# Eval("ClientId2") %>'
							    WeekOrdinal='1' />
                        </div>
                        <div class="span-3 border cent">
						    <rga:Alloc ID="Week2Alloc" runat="server" 
							    AllocationId='<%# Eval("Week2AllocId") %>' 
							    JobId='<%# Eval("JobId") %>' Minutes='<%# Eval("Week2AllocMins") %>' 
							    Note='<%# Eval("Week2AllocNote") %>'
							    UserId="<%# _EmpId %>" UserType="<%# _UserType %>" WeekNumber="<%# _Weeks[1] %>"
							    Start='<%# Eval("StartDate") %>' End='<%# Eval("EndDate") %>'  EmpNoteClientId='<%# Eval("ClientId2") %>'
							    WeekOrdinal='2'/>
                        </div>
                        <div class="span-3 border cent">
						    <rga:Alloc ID="Week3Alloc" runat="server" 
							    AllocationId='<%# Eval("Week3AllocId") %>' 
							    JobId='<%# Eval("JobId") %>' Minutes='<%# Eval("Week3AllocMins") %>' 
							    Note='<%# Eval("Week3AllocNote") %>'
							    UserId="<%# _EmpId %>" UserType="<%# _UserType %>" WeekNumber="<%# _Weeks[2] %>"
							    Start='<%# Eval("StartDate") %>' End='<%# Eval("EndDate") %>' EmpNoteClientId='<%# Eval("ClientId2") %>' 
							    WeekOrdinal='3'/>                                          
                        </div>
                        <div class="span-3 border cent last">                                
						    <rga:Alloc ID="Week4Alloc" runat="server" 
							    AllocationId='<%# Eval("Week4AllocId") %>' 
							    JobId='<%# Eval("JobId") %>' Minutes='<%# Eval("Week4AllocMins") %>' 
							    Note='<%# Eval("Week4AllocNote") %>'
							    UserId="<%# _EmpId %>" UserType="<%# _UserType %>" WeekNumber="<%# _Weeks[3] %>"
							    Start='<%# Eval("StartDate") %>' End='<%# Eval("EndDate") %>' EmpNoteClientId='<%# Eval("ClientId2") %>' 
							    WeekOrdinal='4'/>
                        </div>
                    </div>
                    <!-- JOBLIST ENDS -->
				    </ItemTemplate>
			    </asp:Repeater>
			</div>
            <!-- CLIENT ROW ENDS -->
		</ItemTemplate>
	</asp:Repeater>                 
	</div>

	<div id="table-below">
		<div class="clear">&nbsp;</div> 
		<div id="table-guide">
			<img src="Img/icn_overAllocated.gif" width="9" height="9" alt="Over Allocated" title="Over Allocated" />Over Allocated
			<img src="Img/icn_underAllocated.gif" width="9" height="9" alt="Under Allocated" title="Under Allocated" class="icon-margin" />Under Allocated
		</div>
		<div class="clear">&nbsp;</div>
	</div>

	<div id="popupanel" class="modDig" style="position: fixed; left: 200px; width: 365px; top: 120px; height: 395px; display: none; background-color: #f2f2f2;">
		<div style="width: 365px; background-color: #f2f2f2;">
		<span style="float: right">&nbsp;<a href="javascript:;" onclick="hidepopup();" class="tbox"
			style="width: 15px;"><img src="Img/icon_close.gif" alt="Close Window" width="12px" height="11px" style="padding: 9px 11px 0 0;"/></a></span>
		</div>
		<iframe id="popupiframe" frameborder="0" width="360px" height="370px" style="border: none 0px #f2f2f2">Loading...</iframe>
	</div>

	<div id="notesPanel" class="modDig" style="display: none;">
		<div>
			<span>&nbsp;<a href="javascript:;" onclick="hideNotesPopup();" class="tbox"style="width: 15px;"><img src="Img/icon_close.gif" alt="Close Window" width="12px" height="11px" style="padding: 9px 11px 0 0;"/></a></span>
		</div>
		<iframe name="notesIFrame" id="notesIFrame" frameborder="0" style="border: none 0px #f2f2f2; background-color: #f2f2f2;"></iframe>
	</div>

    </form>
</div>

<rga:Footer ID="tfooter" runat="server" />
<RGAm:Lev2Nav ID="levbNav" runat="server" />
    <script src="Js/jquery-1.2.6.min.js" type="text/javascript"></script>
    <script src="Js/firebug.js" type="text/javascript"></script>
	<script src="Js/shared.js" type="text/javascript"></script>
    <script src="Js/note.js" type="text/javascript"></script>
    <script src="Js/toggle.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
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
        var weekOrdinal = parts[5];
        
        if (IsQuarterHourIncrement($(this)))
        {
            var num = parseFloat($(this).val());
            var HoursDifference = num - CurrentHours;
            HoursDifference = parseFloat(HoursDifference);

            if (HoursDifference != 0)
            {	
                var totals = null;
                if (weekOrdinal == 1)
                    totals = document.getElementById('repClients_ctl00_ltrWeek1Avail');
                else if (weekOrdinal == 2)
                    totals = document.getElementById('repClients_ctl00_ltrWeek2Avail');
                else if (weekOrdinal == 3)
                    totals = document.getElementById('repClients_ctl00_ltrWeek3Avail');
                else if (weekOrdinal == 4)
                    totals = document.getElementById('repClients_ctl00_ltrWeek4Avail');
                var used = totals.innerHTML.substring(totals.innerHTML.indexOf('/')+1);
                used = parseFloat(used);
                var available = totals.innerHTML.substring(0,totals.innerHTML.indexOf('/'));
                available = parseFloat(available);
                used = used + HoursDifference;
                totals.innerHTML = available + '/' + used;
                if (used > available)
                    totals.className = 'over';
                else if (used < available)
                    totals.className = 'under';
                else
                    totals.className = '';
                        
                //update DB
                var textbox = $(this);
                var url = "Services/UpdateAllocation.aspx?allocationid=-1&mins=" + num * 60 + "&jobid=" + jobId + "&employeeid=" + employeeId + "&week=" + weekNum;
                $.get(url, function(data){
                    newAllocId = eval((data))[0].AllocId;
                    noteLen = eval((data))[0].NoteLength;
                    //try to change the id of this allocation's note-button, and change the button's visibility
                    var imgBtn = $(textbox).siblings('.img');
                    $(imgBtn).attr('id','allocNote_' + newAllocId + '_true');
                    $(imgBtn).unbind('click');
                    if (num > 0)
                    {
                        $(imgBtn).click(function() {
                            showNotesPopup(newAllocId,true); 
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
                        $(imgBtn).unbind('click');
                        $(imgBtn).css('cursor','default');
                        $(imgBtn).attr('src','Img/c.gif');
			        }
                });
            }
        }
    });
    
});
    </script>
</body>
</html>
