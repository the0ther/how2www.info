<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeptAllocation.aspx.cs" Inherits="Allocations.DeptAllocation" Trace="false" EnableViewState="true" %>
<%@ Register TagPrefix="RGAWC" Namespace="RGA.UI.WebControls" Assembly="RGA.UI.WebControls" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="topHeader" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="Footer" %>
<%@ Register TagPrefix="RGAm" Namespace="Allocations" Assembly="topNav" %>
<%@ Register TagPrefix="rga" TagName="WeekPicker" Src="~/Controls/WeekPicker.ascx" %>
<html>
<head runat="server">
    <title>Department Allocations</title>
    <link rel="stylesheet" href="Css/Allocations.css" type="text/css" /> 
    <link rel="stylesheet" href="Css/Common.css" type="text/css" />
    <link rel="Stylesheet" href="Css/Popup.css" type="text/css" />    
    <link rel="stylesheet" href="css/blueprint/screen.css" type="text/css" media="screen, projection">
    <link rel="stylesheet" href="css/blueprint/print.css" type="text/css" media="print">    
    <!--[if IE]><link rel="stylesheet" href="css/blueprint/ie.css" type="text/css" media="screen, projection"><![endif]-->
	<style type="text/css">
	#MainGrid tbody tr.header-row, #MainGrid tbody tr.header-row th { background-color: #808080 }
	#MainGrid tbody th.header-name { color: #ffffff; text-align: left; } 
	#MainGrid tbody th, #MainGrid tbody td { border:1px solid #CBCBCB; height:25px; width: 50px !important; }
	span.img { float: right; position: relative; }
	#MainGrid td.info-name { font-weight:bold; text-align:left; width:370px !important; }
	tr.header-row > th, tr.header-row > td { text-align: center; padding: 4px 5px 4px 5px; }
	tr.header-row td, tr.rowalt td { font-size: 8pt; text-align: center; padding: 0 0 0 0;}
	tr.main-row > td.info-name > span { padding-left: 3px; }
	tr.rowalt > td.info-name > a { padding-left: 20px; }
	img.img { margin-top: -4px; }
    #main-info .main-row td, #MainGrid .main-row td, table.table-content .main-row td { background-color:#E5E5E5; }
    /* the letter-spacing style below fixes the problem where table cells become too large in IE and throw off the alignment of note button */
    tr.header-row th a { font-size: 8pt; letter-spacing: -0.5pt; } 
	</style>
	<!--[if IE]>
	<style type="text/css">
	span.hrs { margin-left: 4px; }
	img.img { position: relative; }
    #notesPanel { height: 330px; }
    #notesIFrame { height: 305px; }	
	</style>
	<![endif]-->
</head>
<body>
    <rga:TopHeader ID="theader" pageID="377" ptitle="R/GA Management Tools: Time Tracking"
        runat="server" />
    <RGAm:Lev0Nav ID="aheader" runat="server" PageID="192" lev1NavID="levaNav" lev2NavID="levbNav" />
    <RGAm:Lev1Nav ID="levaNav" runat="server" />
    
	<div id='content' class="contentArea">    
    <form id="form1" runat="server">
		<div id="controls">
			<div class="controls-section">
				<asp:DropDownList ID="DeptsDrop" AutoPostBack="true" runat="server" DataSourceID="DeptsDS"
					DataTextField="Name" DataValueField="DeptId" OnDataBound="DeptsDrop_DataBound"
					OnSelectedIndexChanged="Depts_SelectedIndexChanged" EnableViewState="false"/>
			</div>

			<div class="controls-section border2">
				<strong>Resources</strong>
				<asp:RadioButtonList ID="radResourceOptions" runat="server" AutoPostBack="true" RepeatColumns="2"
						OnSelectedIndexChanged="Resources_SelectedIndexChanged" CssClass="tight"
                        EnableViewState="false">
					<asp:ListItem Value="ASSIGNED" Selected="True">All</asp:ListItem>
					<asp:ListItem Value="ALLOCATED">Allocated</asp:ListItem>
					<asp:ListItem Value="UNALLOCATED">Unallocated</asp:ListItem>
					<asp:ListItem Value="OVERALLOCATED">Over allocated</asp:ListItem>
					<asp:ListItem Value="UNDERALLOCATED">Under allocated</asp:ListItem>
				</asp:RadioButtonList>
			</div>

			<div class="controls-section border2">
				<asp:DropDownList ID="ClientsDrop" runat="server" DataSourceID="ClientsDS"
					DataTextField="Name" DataValueField="ClientId" OnDataBound="Clients_DataBound"
					OnSelectedIndexChanged="Clients_SelectedIndexChanged" AutoPostBack="true" 
					EnableViewState="false"/>

				<asp:DropDownList ID="TitlesDrop" runat="server" DataSourceID="TitlesDS"
					DataTextField="TitleName" DataValueField="TitleName" OnDataBound="Titles_DataBound"
					OnSelectedIndexChanged="Titles_SelectedIndexChanged" AutoPostBack="true" 
					EnableViewState="false"/>
					
				<asp:DropDownList ID="RegionsDrop" runat="server" OnSelectedIndexChanged="RegionsDrop_SelectedIndexChanged" 
					AutoPostBack="true" EnableViewState="false" DataSourceID="RegionsDS" 
					DataTextField="rName" DataValueField="rid" OnDataBound="Regions_DataBound" />
			</div>

			<div id="allocation-date-selection">
				<rga:WeekPicker ID="WeekPicker1" runat="server" EnableViewState="true" />
			</div>
		</div>

		<asp:Table ID="MainGrid" runat="server" CellSpacing="0" CellPadding="0" EnableViewState="false"></asp:Table>


    <asp:SqlDataSource ID="DeptsDS" runat="server" ConnectionString="<%$ConnectionStrings:conn%>"
		SelectCommand="SELECT DeptId, Name FROM AllDepartments WHERE normBillsTime=1 ORDER BY Name"
		SelectCommandType="text" />

	<asp:SqlDataSource ID="ClientsDS" runat="server" ConnectionString="<%$ConnectionStrings:conn%>"
		SelectCommand="SELECT ClientID, Name FROM AllActiveClients ORDER BY Name"
		SelectCommandType="Text" />

    <asp:SqlDataSource ID="RegionsDS" runat="server" ConnectionString="<%$ConnectionStrings:conn %>"
        SelectCommand="SELECT r.OfficeRegionId AS rid, r.Name AS rName FROM BldOfficeRegions AS r ORDER BY DisplayOrder,rid"
        SelectCommandType="Text" />

	<asp:SqlDataSource ID="TitlesDS" runat="server" ConnectionString="<%$ConnectionStrings:conn%>"
		SelectCommand="SELECT TitleName FROM JobTitles WHERE DeptId=@dept_id"
		SelectCommandType="text">
		<SelectParameters>
			<asp:ControlParameter ControlID="DeptsDrop" PropertyName="SelectedValue" Name="dept_id" Type="Int32" />
		</SelectParameters>
	</asp:SqlDataSource>
	<asp:HiddenField ID="WeekNumber" runat="server" EnableViewState="true" />
	
	<div id="table-below">
		<div class="clear">&nbsp;</div> 
		<div id="table-guide">
			<img src="Img/icn_overAllocated.gif" width="9" height="9" alt="Over Allocated" title="Over Allocated" />Over Allocated
			<img src="Img/icn_underAllocated.gif" width="9" height="9" alt="Under Allocated" title="Under Allocated" class="icon-margin" />Under Allocated
		</div>
		<div class="clear">&nbsp;</div>
	</div>
	
    <div id="notesPanel" class="modDig" style="display: none;">
        <div>
            <span style="float: right">&nbsp;<a href="javascript:;" onclick="hideNotesPopup();" class="tbox" style="width: 15px;"><img src="Img/icon_close.gif" alt="Close Window" width="12px" height="11px" style="padding: 9px 11px 0 0;"/></a></span>
        </div>
        <iframe name="notesIFrame" id="notesIFrame" frameborder="0" style="border: none 0px #f2f2f2; background-color: #f2f2f2;">Loading...</iframe>
    </div>

    <asp:HiddenField ID="togglesRecord" runat="server" EnableViewState="true" />
    </form>
    </div>
    <rga:Footer ID="tfooter" runat="server" />
    <RGAm:Lev2Nav ID="levbNav" runat="server" />
    <script src="Js/jquery-1.2.6.min.js" type="text/javascript"></script>
	<script src="Js/jquery.dimensions.min.js" type="text/javascript"></script>
    <script src="Js/popup.js" type="text/javascript"></script>
	<script src="Js/note.js" type="text/javascript"></script>
    <script src="Js/firebug.js" type="text/javascript"></script>
    <script src="Js/shared.js" type="text/javascript"></script>
    <script src="Js/toggle.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

		$(document).ready(function(){
		    console.log('inside document.ready');
		    var p = null;
		
			var week = $('#WeekNumber').val();
			
			
<%
	//this is to disallow non-admin users access to the edit form
	if (_secType == RGA.Allocations.Web.SecurityType.EditAll)	
	{
%>
			$('#MainGrid tr.rowalt td[@class!=info-name] span[@class!=img]').click(function(){
			//$('.hrs').click(function(){
			    console.log('inside the click() handler for span.hrs');
				var id = $(this).attr('id').split('_');
				var user = id[1];
				var job = id[2];

                if (p == null)
                {
                    //console.log('opening the dept alloc editor...');
				    p = new Popup({
					    pWidth: 300, pHeight: 280, iWidth: 290, iHeight: 250, 
					    src: 'DeptAllocEditor.aspx?UserId=' + user + '&JobId=' + job + '&WeekNumber=' + week, 
					    reload: false, visible: true, multiClose: true
				    });
                }
                else
                {
                    p.src = 'DeptAllocEditor.aspx?UserId=' + user + '&JobId=' + job + '&WeekNumber=' + week;
                    p.init();
                }
	        
	            $('img.close-popup').click(function() {
                    window.parent.$('div#block').hide();		
                    window.parent.$('div#popup').hide();
	            });

				p.open();
				return false;
			});
<%
	}
%>
    	    $('span.img').click(function() {
                //look at first sibling, a span, see if it's contents are non-zero, if not, no event handler...
                var val = $(this).siblings('span.hrs').html();
                if (parseFloat(val) != NaN && parseFloat(val) > 0)
                {
                    var id = $(this).children().attr('id');
                    id = id.substring(id.indexOf('_')+1,id.lastIndexOf('_'));
                    showNotesPopup(id,true);
                }
	        });	
        });
	</script>
</body>
</html>