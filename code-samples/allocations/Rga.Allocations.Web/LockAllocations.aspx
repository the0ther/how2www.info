<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LockAllocations.aspx.cs" Inherits="Allocations.LockAllocations" %>
<%@ Register TagPrefix="RGAWC" Namespace="RGA.UI.WebControls" Assembly="RGA.UI.WebControls" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="topHeader" %>
<%@ Register TagPrefix="RGA" Namespace="Allocations" Assembly="Footer" %>
<%@ Register TagPrefix="RGAm" Namespace="Allocations" Assembly="topNav" %>
<html>
<head runat="server">
    <title>Lock Allocations</title>
    <link rel="stylesheet" href="Css/Allocations.css" type="text/css" />
    <link rel="stylesheet" href="Css/Common.css" type="text/css" />
    <style type="text/css" >        
        #LocksGrid { width: 600px; }
        #LocksGrid th {background-color: #808080; color: #ffffff; height: 25px; }
        #LocksGrid td { text-indent: 12px; }
        #LocksGrid td.col1 { text-indent: 12px; width: 400px; }
        #LocksGrid td.col2 { text-indent: 12px; width: 150px; }
        #LocksGrid td.col3 { text-indent: 0px; width: 50px; text-align: center; }
        table { border:1px solid #CBCBCB; }
        td,th { padding: 5px; }
        th.clientName { text-align:left; }
        .fr { float: right; }
    </style>
</head>
<body>
    <rga:TopHeader ID="theader" pageID="378" ptitle="R/GA Management Tools: Time Tracking"
        runat="server" visible="true" />
    <RGAm:Lev0Nav ID="aheader" runat="server" PageID="192" lev1NavID="levaNav" lev2NavID="levbNav" />
    <RGAm:Lev1Nav ID="levaNav" runat="server" />
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smManage" runat="server" />
    <div id='content' class="contentArea">
    <br />
    <div style="width: 600px;">
        <span class="fr"><asp:LinkButton ID="UnlockAll" runat="server" OnClick="Unlock_OnClick">Unlock All</asp:LinkButton> | <asp:LinkButton ID="LockAll" runat="server" OnClick="Lock_OnClick">Lock All</asp:LinkButton></span>
		<asp:Label ID="Week" runat="server"/>
		<asp:HiddenField ID="WeekNumber" runat="server" />
    </div>
    <br />
    <asp:UpdatePanel ID="upManagePanel" UpdateMode="Conditional" runat="server">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="LocksGrid" />
    </Triggers>
    <ContentTemplate>
        <asp:GridView ID="LocksGrid" runat="server" DataSourceID="LocksDS" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Client Name" HeaderStyle-CssClass="clientName" ItemStyle-CssClass="col1 laCol" />
                <asp:BoundField DataField="TotalHours" HeaderText="Total Hours Allocated" ItemStyle-HorizontalAlign="center" ItemStyle-CssClass="col2" />
                <asp:TemplateField HeaderText="Locked" ItemStyle-HorizontalAlign="center" ItemStyle-CssClass="col3">
                    <ItemTemplate>
                        <asp:ImageButton ID="btn" runat="server" OnCommand="btn_OnCommand" CommandArgument='<%# Eval("ClientId") %>' CommandName="lock" ImageUrl='<%# (Convert.ToInt32(Eval("locked"))>0) ? "~/Img/icon_add_on.gif" : "~/Img/icon_add_off.gif" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </ContentTemplate>
    </asp:UpdatePanel>
    </div>
    </form>
    <rga:Footer ID="tfooter" runat="server" />
    <RGAm:Lev2Nav ID="levbNav" runat="server" />
    <asp:SqlDataSource ID="LocksDS" runat="server" ConnectionString="<%$ConnectionStrings:conn%>"
        SelectCommand="ALOC_SelectLocksList" SelectCommandType="StoredProcedure"
        InsertCommand="ALOC_LockAllocations" InsertCommandType="StoredProcedure"
        DeleteCommand="DELETE FROM ALOC_Locks WHERE WeekNumber=@week_num AND ClientId=@client_id" DeleteCommandType="text">
        <SelectParameters>
            <asp:ControlParameter ControlID="WeekNumber" Name="week_number" Type="Int32" />
        </SelectParameters>
        <InsertParameters>
            <asp:Parameter Name="week_number" Type="Int32" />
            <asp:Parameter Name="client_id" Type="int32" />
        </InsertParameters>
        <DeleteParameters>
            <asp:Parameter Name="week_num" Type="Int32" />
            <asp:Parameter Name="client_id" Type="int32" />        
        </DeleteParameters>
    </asp:SqlDataSource>
    <script src="Js/firebug.js" type="text/javascript"></script>
</body>
</html>
