<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageTeams.aspx.cs" Inherits="ManageTeams" %>
<html>
<head runat="server">
    <title>Manage Teams</title>
    <link rel="stylesheet" href="Css/Allocations.css" type="text/css" />
    <link rel="stylesheet" href="Css/Common.css" type="text/css" />
    <style type="text/css" >
        #managepanel2 
        {
            width: 782px;
            border: solid 1px #e6e6e6; 
            background-color: #f2f2f2;
            font-weight: bold;
            margin-top:5px;
            padding-top:10px;
            padding-left:10px;
        }
    
        .ListHeader {
            background-color: #808080; color: #ffffff; 
            height:26px;
        }
        
        #TeamGrid {
            margin: 0px 0px 0px 8px; 
            border: solid 1px #333333; 
        }
    
        #ResourcesGrid 
        {
	        background-color: #f2f2f2; 
	        padding:1px;
        }
    
        #ControlsPanel {
            float: right; margin: 0px 8px 8px 0px;
            height:270px;
            width: 350px; vertical-align: top; 
            overflow:auto;
        }


        #ResourcesPanel {
            width: 375px;
            margin: -55 8px 0px 8px; 
            vertical-align: top;
            overflow:auto;
	        display:none;
        }
        
        #GroupName { margin: 0 0 8px 8px; }
            
            /*
        #ResourcesPanel table.hdr {
            margin-top: 10px; background-color: #808080; color: #ffffff; 
            height:25px; 
        }
    */
    
        table.teamgridstyle
        {
            width:350px;
            border: solid 1px #333333; 
        }
    
        a.manageLinks
        {
            font-weight:bold;
            line-height:20px;
            text-decoration:none;
        }
        
        .tgDelMemberCell
        {
            height:28px;
            width:22px;
            cursor:pointer;
        }
        
        .tgNameCell
        {
            width:292px;
        }

        .tgDeptCell
        {
            width:36px;
        }

        .tgClientTitleCell
        {
            width:168px;
        }
        
        .tgClientAddCell
        {
            width:28px;
        }

        .tgClientNameCell
        {
            width:150px;
        }

        td.gridCell
        {
            height:30px;
            border:1px solid #F2F2F2;
            text-align:center;
        }
        
        th
        {
            border:1px solid #CBCBCB;
            border-right:0px;
            margin:0px;
            padding:0px;
        }

        div.overflowpanel
        {
            height:270px;
            overflow:auto;
        }

        div.btn
        {
            border:1px solid #CDCDCD;
            display:inline;
            padding:4px;
            padding-left:8px;
            padding-right:8px;
            background-color:#E6E6E6;
            cursor:pointer;
        }
        
        td.mainColumns
        {
            width:380px;
        }
        
        td.hdrCells
        {
            border:solid 1px #F2F2F2;
            border-bottom:solid 1px #000;
            background-color:#F2F2F2;
        }
    </style>
    <!--[if IE 7]>
    <style type="text/css" >
        td.gridCell
        {
            height:26px;
            border:1px solid #F2F2F2;
            text-align:center;
        }
    </style>
    <![endif]-->

</head>
<body style="background-color: #f2f2f2;">
    <form id="form1" runat="server">
    <div style="background-color: #f2f2f2;">
<div id="managepanel2" style="background-color:#F2F2F2;" >
<asp:Panel ID="ManagePanel" runat="server" >
    <table border="0" style="background-color:#F2F2F2;" cellpadding="0" cellspacing="0" >
        <tr style="vertical-align:top;" >
            <td class="mainColumns hdrCells" >
                <div style="height:10px;">
    	            <asp:Label ID="GroupName" runat="server" Text="" CssClass="blk" style="margin-left:8px;font-size:18px;" />
                </div>
            </td>
            <td style="width:3px;background-color:#F2F2F2;" />
            <td style="padding-top:3px;padding-bottom:5px;" class="mainColumns hdrCells" >
                    Client Group: <asp:DropDownList ID="Groups" runat="server" CssClass="tbox" />                    
            </td>
        </tr>
        <tr>
            <td style="background-color:#F2F2F2; height:34px;" />
            <td style="background-color:#F2F2F2;" rowspan="2" />
            <td style="background-color:#F2F2F2; padding-top:5px; padding-bottom:5px; " >
                    <span class="blk" style="display:none;" id="ResourcesDropDownCell" >Add client resources: 
                    <asp:DropDownList ID="Depts" runat="server" CssClass="tbox" /></span>
            </td>
        </tr>
        <tr style="vertical-align:top;" >
            <td style="background-color:#F2F2F2;" >
                <div class="overflowpanel" >
                    <table id="TeamGrid" class="teamgridstyle" cellspacing="0" >
                        <thead class="ListHeader" >
                            <th class="tgDelMemberCell" ></th>
                            <th class="tgNameCell" >Members</th>
                            <th style="border-right:1px solid #CBCBCB;" class="tgDeptCell" >Dept</th>
                        </thead>
                        <tbody>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                            <tr><td class="gridCell tgDelMemberCell" >&nbsp;</td><td class="gridCell tgNameCell"  >&nbsp;</td><td class="gridCell tgDeptCell"  >&nbsp;</td></tr>
                        </tbody>
                    </table>
                </div>
            </td>
            <td style="background-color:#F2F2F2;padding-bottom:10px;" >
                <div id="ControlsPanel" class="overflowpanel" >
		            <div id="GroupNotesSection" >
                        <div style="height:10px; font-weight:normal;">
                            Note: <asp:Label ID="GroupNote" runat="server" />
		                </div>
		            </div>
		            <br />
                    <a href="javascript:;" id="AddMembers" style="color: #000000;" class="manageLinks" >Add Members</a><br />
                    <a href="javascript:;" id="EditGroup" style="color: #000000;" class="manageLinks" >Edit Group</a><br />
                    <a href="javascript:;" id="DeleteGroup" style="color: #000000;" class="manageLinks" >Delete Group</a><br />
                    <hr />
                    <a href="javascript:;" id="CreateGroup" class="manageLinks" style="color:#C00;" >Create New Group</a><span id="editGroupHeader" style="color:#333;display:none;" >Editing Existing Group:</span><br />
                    
                    <div id="NewGroupCtrls" style="display: none;">
                        <br />
                        <asp:Label ID="EnterName" runat="server">Group name</asp:Label><br />
                        <span style="line-height:5px;" ><br /></span>
                        <asp:TextBox ID="NewGroupName" runat="server" /><br /><br />
                        <asp:Label ID="EnterNote" runat="server">Note or description</asp:Label><br />
                        <asp:TextBox ID="NewGroupNote" runat="server" Width="325px" /><br /><br />
                        <div>
                            <div id="NewGroupNext" class="btn" >Next</div>&nbsp;
                            <div id="NewGroupCancel" class="btn" >Cancel</div>
                        </div>
                        <br />
                    </div>
	            </div>
               
                <div id="ResourcesPanel" class="overflowpanel" >
                    <table id="ResourcesGrid" border="0" cellspacing="0" class="teamgridstyle">
                        <thead class="ListHeader" >
                            <th class="tgClientNameCell" >Name</th>
                            <th class="tgClientTitleCell" >Title</th>
                            <th style="border-right:1px solid #CBCBCB;" class="tgClientAddCell" >Add</th>
                        </thead>
			            <tbody />
		            </table>
	            </div>
                <br />
                <div id="Done" class="btn" style="display:none;">Done</div><br />
            </td>
        </tr>
        <tr>
			<td colspan="3" style="background-color: #f2f2f2;"></td>
        </tr>
    </table>
</asp:Panel>
</div>
    </div>
    </form>
    <script src="Js/jquery-1.2.6.min.js" type="text/javascript"></script>
    <script src="Js/teams.js" type="text/javascript"></script>
    <script src="Js/firebug.js" type="text/javascript"></script>
</body>
</html>
