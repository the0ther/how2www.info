<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegionAllocation.aspx.cs" Inherits="Allocations.RegionAllocation" %>
<%@ Register TagPrefix="RGAWC" Namespace="RGA.UI.WebControls" Assembly="RGA.UI.WebControls" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="topHeader" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="Footer" %>
<%@ Register TagPrefix="RGAm" Namespace="Allocations" Assembly="topNav" %>
<%@ Register TagPrefix="rga" TagName="WeekPicker" Src="~/Controls/WeekPicker.ascx" %>
<%@ Register TagPrefix="rga" TagName="Alloc" Src="~/Controls/TeamAllocationWithNote.ascx" %>
<html>
<head runat="server">
    <title>Region Allocations</title>
    <link rel="stylesheet" href="Css/Allocations.css" type="text/css" /> 
    <link rel="stylesheet" href="Css/Common.css" type="text/css" />
    <link rel="Stylesheet" href="Css/Popup.css" type="text/css" />    
    <link rel="stylesheet" href="css/blueprint/screen.css" type="text/css" media="screen, projection">
    <link rel="stylesheet" href="css/blueprint/print.css" type="text/css" media="print">    
    <!--[if IE]><link rel="stylesheet" href="css/blueprint/ie.css" type="text/css" media="screen, projection"><![endif]-->
    <style type="text/css">
    #Regions { width: 150px; }
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
        <asp:HiddenField ID="togglesRecord" runat="server" EnableViewState="true" />
        
        <div id="allocation-controls">
            <div class="controls-section">
                <asp:DropDownList ID="Regions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Regions_SelectedIndexChanged" />
            </div>
            <div class="controls-section border2">
			    <p>
				    <strong>Resources</strong>
			    </p>
			    <asp:RadioButtonList ID="ResourceOptions" runat="server" AutoPostBack="true" RepeatColumns="2"
				    OnSelectedIndexChanged="ResourceOptions_SelectedIndexChanged" CssClass="tight">
			        <asp:ListItem Value="ASSIGNED" Selected="True">Assigned</asp:ListItem>
			        <asp:ListItem Value="ALLOCATED">Allocated</asp:ListItem>
			        <asp:ListItem Value="UNALLOCATED">Unallocated</asp:ListItem>
			        <asp:ListItem Value="OVERALLOCATED">Over allocated</asp:ListItem>
			        <asp:ListItem Value="UNDERALLOCATED">Under allocated</asp:ListItem>
			    </asp:RadioButtonList>
			    <span class="msg">for current week</span>
		    </div>
    		<div class="controls-section border2">
            	<asp:DropDownList ID="Department" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Department_SelectedIndexChanged" />
			</div>
        </div>
        
        <div id="allocation-date-selection">
            <rga:WeekPicker ID="ctlWeekPicker" runat="server" ShowDoubleArrows="true" />
        </div>
        
        <div id="project-allocations" class="container">
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
    
            
        	<asp:Repeater ID="Resources" runat="server" EnableViewState="true" OnItemDataBound="Resources_ItemDataBound">
            <ItemTemplate>
                <div class="rowmain deptrow span-24 last">
                    <div class="span-9 left border first">
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
                        <div class="span-1 cent">&nbsp;</div>
			            <div class="span-1 cent"><%#Eval( "Week1AvailableHours" )%>/<%#RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week1AllocatedHoursTotal" )))%></div>
			            <div class="span-1 cent last">&nbsp;</div>
                    </div>
                    <div class="span-3 border cent ">
                        <div class="span-1 cent">&nbsp;</div>
                        <div class="span-1 cent"><%#Eval( "Week2AvailableHours" )%>/<%#RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week2AllocatedHoursTotal" ))) %></div>
                        <div class="span-1 cent last">&nbsp;</div>
                    </div>
                    <div class="span-3 border cent ">
                        <div class="span-1 cent">&nbsp;</div>
                        <div class="span-1 cent"><%#Eval( "Week3AvailableHours" )%>/<%#RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week3AllocatedHoursTotal" ))) %></div>
                        <div class="span-1 cent last">&nbsp;</div>
                    </div>
                    <div class="span-3 border cent last ">
                        <div class="span-1 cent">&nbsp;</div>
                        <div class="span-1 cent"><%#Eval( "Week4AvailableHours" )%>/<%#RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (Eval( "Week4AllocatedHoursTotal" ))) %></div>
                        <div class="span-1 cent last">&nbsp;</div>
                    </div>
                    <asp:Repeater ID="Jobs" runat="server" EnableViewState="true">
		            <ItemTemplate>
                    <!-- JOBLIST FOR USER ABOVE STARTS -->
                    <div class="rowalt span-24 last">
                        <div class="left span-9 border first">
				            <a class="name" href="ProjectAllocation.aspx?ProjectId=<%#Eval("JobId") %>&Week=<%# _Weeks[0] %>" title="<%# Eval("JobCode") + " - " + HttpUtility.HtmlEncode(Eval( "JobName" ) + ((Eval("StartDate")==DBNull.Value) ? "" : " (" + Convert.ToDateTime(Eval("StartDate")).ToString("MM/dd/yyyy") + " - ") + ((Eval("EndDate")==DBNull.Value) ? "" : Convert.ToDateTime(Eval("EndDate")).ToString("MM/dd/yyyy")) + ")") %>">
				            <%# Eval("ClientShortName").ToString() %> - <%# Eval( "JobName" ).ToString().Length <= 42 ? Eval( "JobName" ).ToString() : Eval( "JobName" ).ToString().Substring(0, 42) + "..."%>
				            </a>
                            <asp:Label ID="UnassignedStar" runat="server" CssClass="red" Visible='<%#( Eval("Assigned").ToString()=="1") ? true : false %>'>*</asp:Label>
                        </div>
                        <div class="span-1 border right">&nbsp;</div>
                        <div class="span-2 border right"><span><%# Eval( "TitleHoursBudgeted" )%> / <%# Eval( "TitleHoursUsed") %></span></div>
                        <div class="span-3 border cent" title="Available / Allocated this client/ Allocated all projects">
				            <rga:Alloc ID="Week1Alloc" runat="server" AllocationId='<%# Eval("Week1AllocationId") %>' 
					            Client='<%# 125 %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
					             JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week1AllocationMins") %>' Note='<%# Eval("Week1AllocNote") %>'
					              UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[0] %>' />
                        </div>
                        <div class="span-3 border cent">
			                <rga:Alloc ID="Week2Alloc" runat="server" AllocationId='<%# Eval("Week2AllocationId") %>' 
				                Client='<%# 125 %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
				                 JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week2AllocationMins") %>' Note='<%# Eval("Week2AllocNote") %>'
				                  UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[1] %>' />
                        </div>
                        <div class="span-3 border cent">
				            <rga:Alloc ID="Week3Alloc" runat="server" AllocationId='<%# Eval("Week3AllocationId") %>' 
					            Client='<%# 125 %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
					             JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week3AllocationMins") %>' Note='<%# Eval("Week3AllocNote") %>'
					              UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[2] %>' />                                        
                        </div>
                        <div class="span-3 border cent last">                                
				            <rga:Alloc ID="Week4Alloc" runat="server" AllocationId='<%# Eval("Week4AllocationId") %>' 
					            Client='<%# 125 %>' End='<%#Eval("EndDate") %>' Start='<%#Eval("StartDate") %>'
					             JobId='<%#Eval("JobId") %>' Minutes='<%#Eval("Week4AllocationMins") %>' Note='<%# Eval("Week4AllocNote") %>'
					              UserId='<%#Eval("UserId") %>' UserType='<%# _secType %>' WeekNumber='<%# _Weeks[3] %>' />
                        </div>
                    </div>                                                                   
                    <!-- JOBLIST ENDS -->
                    </ItemTemplate>
                    </asp:Repeater>
                </div>
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
        $(document).ready(function() {
            var CurrentHours;

            $('input.img').click(function() {
                var parts = $(this).attr('id').split('_');
                if (parts[1] != 0)
                    showNotesPopup(parts[1], parts[2]);
                return false;
            });

            $('input.txt').focus(function() {
                if (IsQuarterHourIncrement($(this)))
                    CurrentHours = $(this).val();
            });

            $('input.txt').blur(function() {
                var newAllocId;
                var noteLen;
                var parts = $(this).attr('id').split('_');
                var weekNum = parts[1];
                var employeeId = parts[2];
                var jobId = parts[3];
                //console.log('weekNum: ' + weekNum + ' employeeId: ' + employeeId + ' jobId: ' + jobId);

                if (IsQuarterHourIncrement($(this))) {
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
                    if (HoursDifference != 0) {
                        var weekAvailableObj = $('#' + weekAvailableId);
                        var weekObj = $('#' + weekId);
                        var weekTotalObj = $('#' + weekTotalId);
                        weekAvailableHours = parseFloat($(weekAvailableObj).text())
                        weekHours = parseFloat($(weekObj).text()) + HoursDifference
                        weekTotalHours = parseFloat($(weekTotalObj).text()) + HoursDifference
                        $(weekObj).text(weekHours);
                        $(weekTotalObj).text(weekTotalHours);
                        if (weekAvailableHours > weekTotalHours) {
                            $(weekAvailableObj).attr('class', "under");
                            $(weekTotalObj).attr('class', "under");
                            $(weekObj).attr('class', "under");
                        }
                        else if (weekAvailableHours < weekTotalHours) {
                            $(weekAvailableObj).attr('class', "over");
                            $(weekTotalObj).attr('class', "over");
                            $(weekObj).attr('class', "over");
                        }
                        else {
                            $(weekAvailableObj).attr('class', "");
                            $(weekTotalObj).attr('class', "");
                            $(weekObj).attr('class', "");
                        }
                        //console.log('about to do the allocation...');
                        //update DB
                        var textbox = $(this);
                        var url = "Services/UpdateAllocation.aspx?allocationid=-1&mins=" + num * 60 + "&jobid=" + jobId + "&employeeid=" + employeeId + "&week=" + weekNum;
                        $.get(url, function(data) {
                            //console.log('made the ajax call setting up results..');
                            newAllocId = eval((data))[0].AllocId;
                            noteLen = eval((data))[0].NoteLength;
                            //console.log('new id is: ' + newAllocId + ' and noteLen is: ' + noteLen);
                            var imgBtn = $(textbox).parent().siblings('.img');
                            $(imgBtn).attr('id', 'allocNote_' + newAllocId + '_true');
                            //console.log('num is : ' + num);
                            if (parseFloat(num) > 0) {
                                //console.log('did the allocation, going to setup the image button now...');
                                //try to change the id of this allocation's note-button, and change the button's visibility
                                $(imgBtn).unbind('click');
                                $(imgBtn).click(function() {
                                    showNotesPopup(newAllocId, true);
                                    return false;
                                });
                                $(imgBtn).css('cursor', 'pointer');
                                if (noteLen <= 0)
                                    $(imgBtn).attr('src', 'Img/icon_note_off.gif');
                                else
                                    $(imgBtn).attr('src', 'Img/icon_note_on.gif');
                            }
                            else {
                                //disable the notes button since this is an allocation with 0 hours
                                $(imgBtn).attr('src', 'Img/c.gif');
                                $(imgBtn).css('cursor', 'default');
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