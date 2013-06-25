<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectAllocation.aspx.cs" Trace="false" Inherits="Allocations.ProjectAllocation" %>
<%@ Register TagPrefix="RGAWC" Namespace="RGA.UI.WebControls" Assembly="RGA.UI.WebControls" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="topHeader" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="Footer" %>
<%@ Register TagPrefix="RGAm" Namespace="Allocations" Assembly="topNav" %>
<%@ Register TagPrefix="rga" TagName="WeekPicker" Src="~/Controls/WeekPicker.ascx" %>
<%@ Register TagPrefix="rga" TagName="Alloc" Src="~/Controls/ProjAllocationWithNote.ascx" %>
<%@ Register TagPrefix="rga" TagName="NameTag" Src="~/Controls/TruncdName.ascx" %>
<html>
<head runat="server">
    <title>Project Allocations</title>
    <link rel="stylesheet" href="Css/Allocations.css" type="text/css" /> 
    <link rel="stylesheet" href="Css/Common.css" type="text/css" />
    <link rel="Stylesheet" href="Css/Popup.css" type="text/css" />    
    <link rel="stylesheet" href="css/blueprint/screen.css" type="text/css" media="screen, projection">
    <link rel="stylesheet" href="css/blueprint/print.css" type="text/css" media="print">    
    <!--[if IE]><link rel="stylesheet" href="css/blueprint/ie.css" type="text/css" media="screen, projection"><![endif]-->
<!--[if IE]>
<style type="text/css">
    #notesPanel {height: 330px; }
    #notesIFrame { height: 305px; }
    input { color: red; }
</style>
<![endif]-->
<style type="text/css">
input.img { position: absolute; margin-left: -3px; }
</style>
</head>
<body>
    <rga:TopHeader ID="theader" pageID="375" ptitle="R/GA Management Tools: Time Tracking"
        runat="server" />
    <RGAm:Lev0Nav ID="aheader" runat="server" PageID="192" lev1NavID="levaNav" lev2NavID="levbNav" />
    <RGAm:Lev1Nav ID="levaNav" runat="server" />
<div id="content" class="contentArea">
    <form id="form1" runat="server">
    <asp:Label ID="JobName" runat="server" />
    <asp:HiddenField ID="togglesRecord" runat="server" EnableViewState="true" />
	<asp:HiddenField ID="rebind" runat="server" EnableViewState="true" />

    <div id="allocation-controls">
        <div class="controls-section">
	        <asp:DropDownList ID="ClientsDrop" runat="server" OnSelectedIndexChanged="ClientsDrop_SelectedIndexChanged" 
			        AutoPostBack="true" Width="250px"/><br />
	        <asp:DropDownList ID="JobsDrop" runat="server" OnSelectedIndexChanged="JobsDrop_SelectedIndexChanged" 
			        AutoPostBack="true" Width="250px"/>
	        <asp:RadioButtonList ID="AllOrAssigned" runat="server" RepeatDirection="horizontal" 
	            OnSelectedIndexChanged="AllOrAssigned_SelectedIndexChanged" AutoPostBack="true" CssClass="tight small">
		        <asp:ListItem Value="ALL">All</asp:ListItem>
		        <asp:ListItem Value="ASSIGNED" Selected="true">Assigned</asp:ListItem>
	        </asp:RadioButtonList>
        </div>
        <div class="controls-section border2">
            <strong>Resources</strong><asp:Literal ID="Pipe" runat="server" Text=" | " Visible="false" />
	        <asp:Literal ID="ManageLink" runat="server" />
	        <asp:RadioButtonList ID="radResourceOptions" runat="server" AutoPostBack="true" RepeatColumns="2"
		        OnSelectedIndexChanged="radResourceOptions_SelectedIndexChanged" CssClass="tight">
		        <asp:ListItem Value="ASSIGNED" Selected="True">Assigned</asp:ListItem>
		        <asp:ListItem Value="ALLOCATED">Allocated</asp:ListItem>
		        <asp:ListItem Value="UNALLOCATED">Unallocated</asp:ListItem>
		        <asp:ListItem Value="OVERALLOCATED">Over allocated</asp:ListItem>
		        <asp:ListItem Value="UNDERALLOCATED">Under allocated</asp:ListItem>
	        </asp:RadioButtonList>
	        <span class="msg">for current week</span>
        </div>
        <div class="controls-section border2">
	        <asp:DropDownList ID="RegionsDrop" runat="server" OnSelectedIndexChanged="RegionsDrop_SelectedIndexChanged" AutoPostBack="true" />
	        <p style="margin: 10px 0 0 0;"><asp:Label ID="ProjDate" runat="server" CssClass="msg" /></p>
        </div>
    </div>
    
    <div class="clear"></div>

    <div id="allocation-date-selection">
        <rga:WeekPicker ID="ctlWeekPicker" runat="server" ShowDoubleArrows="true" />
    </div>

<div id="project-allocations" class="container">
    <!-- TOP LEVEL ROW DARK BACKGROUND STARTS -->
    <div class="head rowmain span-24 last">
        <div class="span-9 title border first left">
            <img class="toggle-all" src="./Img/icn_open.gif" alt="toggle" /><span>Departments</span>
        </div>
        <div class="span-3 cent border" title="Budgeted by title / Used by title">Bdgt/Used</div>
        <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekOne" runat="server">&nbsp;</asp:Literal></strong></div>
        <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekTwo" runat="server">&nbsp;</asp:Literal></strong></div>
        <div class="span-3 cent border"><strong><asp:Literal ID="ltrWeekThree" runat="server">&nbsp;</asp:Literal></strong></div>
        <div class="span-3 cent border last"><strong><asp:Literal ID="ltrWeekFour" runat="server">&nbsp;</asp:Literal></strong></div>
    </div>
    <!-- TOP LEVEL ROW ENDS -->
    
    <!-- THIS GRID IS DEPARTMENTS WITH ALL THE EMPLOYEES LISTED BENEATH -->
    <asp:Repeater ID="repDepts" runat="server" EnableViewState="true" OnItemDataBound="repDepts_ItemDataBound">
        <ItemTemplate>
            <!-- DEPARTMENT ROW STARTS -->
            <div class="rowmain deptrow span-24 last">
                <div class="span-9 left border first">
                    <img id="toggleImage_<%# Eval("DeptId") %>" src="img/icn_open.gif" class="toggle-row" alt="toggle" />
                    <a class="toggle-row" href="javascript:;" id="employee_<%# Eval("DeptId") %>" title="<%# Eval("Name") %>">
                        <%# Eval("Name") %>
                    </a>
                </div>
                <div class="span-3 border">&nbsp;</div>
                <div class="span-3 border cent ">
                    <strong><span id="week_<%#Eval( "Week1" )%>_<%#Eval( "DeptId" )%>"><%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1Total" )))%></span></strong>
                </div>
                <div class="span-3 border cent ">
                    <strong><span id="week_<%#Eval( "Week2" )%>_<%#Eval( "DeptId" )%>">
                                <%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2Total" )))%>
                            </span></strong>
                </div>
                <div class="span-3 border cent ">
                    <strong><span id="week_<%#Eval( "Week3" )%>_<%#Eval( "DeptId" )%>">
                                <%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3Total" )))%>
                            </span></strong>
                </div>
                <div class="span-3 border cent last ">
                    <strong><span id="week_<%#Eval( "Week4" )%>_<%#Eval( "DeptId" )%>">
                                <%# RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4Total" )))%>
                            </span>
                    </strong>
                </div>
                <asp:Repeater ID="repEmps" runat="server" EnableViewState="true">
                    <ItemTemplate>                        
                    <!-- JOBLIST FOR DEPARTMENT ABOVE STARTS -->
                    <div class="rowalt span-24 last">
                        <div class="left span-9 border first">
                            <RGA:NameTag ID="NameTag" runat="server" UserId='<%#Eval("UserId") %>' FirstName='<%# Eval("FirstName")%>' 
                                        LastName='<%# Eval("LastName")%>' Title='<%#Eval("Title") %>' WeekNumber='<%# _Weeks[0] %>' 
                                        Limit="46" />
                            <asp:Label ID="UnassignedStar" runat="server" CssClass="red" Visible='<%#( Eval("Assigned").ToString()=="1") ? true : false %>'>*</asp:Label>
                        </div>
                        <div class="span-3 border right"><span><%# Eval("HoursBudgeted") %> / <%# Eval("HoursUsed") %></span></div>
                        <div class="span-3 border cent">
                            <rga:Alloc ID='Week1Alloc' runat='server' AllocationId='<%# Eval("Week1AllocId") %>'
                                 Client='<%# _ClientId %>' End='<%# _JobEndDate %>' JobId='<%# _JobId %>'
                                  Minutes='<%# Eval("Week1AllocMins") %>' Note='<%# Eval("Week1AllocNote") %>'
                                   Start='<%# _JobStartDate %>' UserId='<%# Eval("UserId") %>' UserType='<%# _UserType %>'
                                   WeekNumber='<%# _Weeks[0] %>' Available='<%#Eval("Week1AvailableHours")%>' 
                                   Total='<%#Eval( "Week1AllocatedHoursTotal" ) %>' />
                        </div>
                        <div class="span-3 border cent">
                            <rga:Alloc ID='Week2Alloc' runat='server' AllocationId='<%# Eval("Week2AllocId") %>'
                                 Client='<%# _ClientId %>' End='<%# _JobEndDate %>' JobId='<%# _JobId %>'
                                  Minutes='<%# Eval("Week2AllocMins") %>' Note='<%# Eval("Week2AllocNote") %>'
                                   Start='<%# _JobStartDate %>' UserId='<%# Eval("UserId") %>' UserType='<%# _UserType %>'
                                   WeekNumber='<%# _Weeks[1] %>' Available='<%#Eval("Week2AvailableHours")%>' 
                                   Total='<%#Eval( "Week2AllocatedHoursTotal" ) %>' />
                        </div>
                        <div class="span-3 border cent">
                            <rga:Alloc ID='Week3Alloc' runat='server' AllocationId='<%# Eval("Week3AllocId") %>'
                                 Client='<%# _ClientId %>' End='<%# _JobEndDate %>' JobId='<%# _JobId %>'
                                  Minutes='<%# Eval("Week3AllocMins") %>' Note='<%# Eval("Week3AllocNote") %>'
                                   Start='<%# _JobStartDate %>' UserId='<%# Eval("UserId") %>' UserType='<%# _UserType %>'
                                   WeekNumber='<%# _Weeks[2] %>' Available='<%#Eval("Week3AvailableHours")%>' 
                                   Total='<%#Eval( "Week3AllocatedHoursTotal" ) %>' />                                            
                        </div>
                        <div class="span-3 border cent last">                                
                            <rga:Alloc ID='Week4Alloc' runat='server' AllocationId='<%# Eval("Week4AllocId") %>'
                                         Client='<%# _ClientId %>' End='<%# _JobEndDate %>' JobId='<%# _JobId %>'
                                          Minutes='<%# Eval("Week4AllocMins") %>' Note='<%# Eval("Week4AllocNote") %>'
                                           Start='<%# _JobStartDate %>' UserId='<%# Eval("UserId") %>' UserType='<%# _UserType %>'
                                           WeekNumber='<%# _Weeks[3] %>' Available='<%#Eval("Week4AvailableHours")%>' 
                                           Total='<%#Eval( "Week4AllocatedHoursTotal" ) %>' />
                        </div>
                    </div>
                    <!-- JOBLIST ENDS -->    
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <!-- DEPARTMENT ROW ENDS -->        
        </ItemTemplate>
    </asp:Repeater>
</div>
   
    <div id="table-below">
            <div class="clear">&nbsp;</div> 
            <div id="table-guide">
                <img src="Img/icn_overAllocated.gif" width="9" height="9" alt="Over Allocated" title="Over Allocated" />Over Allocated
                <img src="Img/icn_underAllocated.gif" width="9" height="9" alt="Under Allocated" title="Under Allocated" class="icon-margin" />Under Allocated
                <span class="divider"></span>Available | <strong>Allocated this project</strong> | Total 
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
        <div style="width: 245px; background-color: #f2f2f2;">
        <span style="float: right">&nbsp;<a href="javascript:;" onclick="hideNotesPopup();" class="tbox"
            style="width: 15px;"><img src="Img/icon_close.gif" alt="Close Window" width="12px" height="11px" style="padding: 9px 11px 0 0;"/></a></span>
        </div>
        <iframe name="notesIFrame" id="notesIFrame" frameborder="0" width="245px" height="275px" style="border: none 0px #f2f2f2; background-color: #f2f2f2;">Loading...</iframe>
    </div>

    </form>
</div>
    <rga:Footer ID="tfooter" runat="server" />
    <RGAm:Lev2Nav ID="levbNav" runat="server" />
    <script src="Js/jquery-1.2.6.min.js" type="text/javascript"></script>
	<script src="Js/jquery.dimensions.min.js" type="text/javascript"></script>
    <script src="Js/note.js" type="text/javascript"></script>
	<script src="Js/firebug.js" type="text/javascript"></script>
	<script src="Js/popup.js" type="text/javascript"></script>
	<script src="Js/shared.js" type="text/javascript"></script>
    <script src="Js/toggle.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
    function openManagePanel()
    {
	    var client = $('#dropClient option:selected').val();

	    var p = new Popup({
		    pWidth: 824, pHeight: 454, iWidth: 815, iHeight: 420, src: 'ManageAssignments.html?ClientId=' + client 
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
        var weekOrdinal = parts[5];
        
        if (IsQuarterHourIncrement($(this)))
        {
            var num = parseFloat($(this).val());
            var weekAvailableId = "weekavailable_" + weekNum + "_" + employeeId;
            var weekId = "week_" + weekNum + "_" + employeeId;
            var weekTotalId = "weektotal_" + weekNum + "_" + employeeId;
            var deptId = $(this).parent().parent().parent().parent().children().eq(0).children('img.toggle-row').attr('id');
            deptId = deptId.substring(deptId.indexOf('_')+1,deptId.length);
            var deptTotalId = "week_" + weekNum + "_" + deptId;

            var weekAvailableHours = 0;
            var weekHours = 0;
            var weekTotalHours = 0;
            var deptTotalHours = 0;

            var HoursDifference = num - CurrentHours;

            if (HoursDifference != 0)
            {
                var weekAvailableObj = $('#' + weekAvailableId); 
                var weekObj = $('#' + weekId); 
                var weekTotalObj = $('#' + weekTotalId); 
                var deptTotalObj;
                deptTotalObj = $('#' + deptTotalId);
                weekAvailableHours = parseFloat($(weekAvailableObj).text())
                weekTotalHours = parseFloat($(weekTotalObj).text()) + HoursDifference
                deptTotalHours = parseFloat($(deptTotalObj).text()) + HoursDifference;
                $(weekTotalObj).text(weekTotalHours);
                $(deptTotalObj).text(deptTotalHours);

                if (weekAvailableHours > weekTotalHours)
                {
                    $(weekAvailableObj).addClass('under');
                    $(weekAvailableObj).removeClass('over');
                    $(weekTotalObj).addClass('under');
                    $(weekTotalObj).removeClass('over');
                }
                else if (weekAvailableHours < weekTotalHours)
                {
                    $(weekAvailableObj).addClass("over");
                    $(weekTotalObj).addClass("over");
                    $(weekAvailableObj).removeClass("under");
                    $(weekTotalObj).removeClass("under");
                }
                else
                {
                    $(weekAvailableObj).removeClass("over");
                    $(weekTotalObj).removeClass("over");
                    $(weekAvailableObj).removeClass("under");
                    $(weekTotalObj).removeClass("under");
                }
                    
                //update DB
                var textbox = $(this);
                var url = "Services/UpdateAllocation.aspx?allocationid=-1&mins=" + num * 60 + "&jobid=" + jobId + "&employeeid=" + employeeId + "&week=" + weekNum;
                $.get(url, function(data) {
                    newAllocId = eval((data))[0].AllocId;
                    noteLen = eval((data))[0].NoteLength;
                    var imgBtn = $(textbox).parent().siblings('.img');
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
