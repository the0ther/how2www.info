<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ page language="C#" autoeventwireup="true" CodeFile="AllocationNote.aspx.cs" inherits="Allocations.AllocationNote" %>
<html>
<head runat="server">
    <link href="Css/Allocations.css" type="text/css" rel="stylesheet"/>
    <link href="Css/Common.css" type="text/css" rel="stylesheet"/>
    <title>Allocation Note</title>
    <link rel="stylesheet" href="css/blueprint/screen.css" type="text/css" media="screen, projection">
    <link rel="stylesheet" href="css/blueprint/print.css" type="text/css" media="print">    
    <!--[if IE]><link rel="stylesheet" href="css/blueprint/ie.css" type="text/css" media="screen, projection"><![endif]-->
    <style type="text/css">
    #noteContent { background-color: #f2f2f2; padding: 0px 0 0 0; width: 185px; }
    #noteContent h2 { margin: 0 0 5px 15px; font-size: 12pt; font-family: Verdana; }
    #noteContent p {margin: 15px 0 4px 15px; }
    #noteBody { background-color: #f2f2f2; }
    #note_ReadOnlyNote { margin: 0 0 0 10px; }
    input.tbox { padding: 4px 8px 4px 8px; text-align: center; font-weight: bold; width: 52px; height: 23px; font-size: 8pt; }
    #Note { margin: 0 0 5px 15px; height:80px; width: 200px; }
    #radBtns { margin: 0 0 5px 15px; width: 185px; }
    #radBtns td { background-color: #f2f2f2; padding: 3px 3px 3px 3px; width: 30px; }
    #radBtns input { margin: 0 3px 0 0; }
    #Ok { margin: 0 0 0 160px; }
    p strong { font-size: 8pt; }
    label { font-weight: normal; font-size: 8pt; }
    </style>
    
</head>
<body id="noteBody">
    <form id="form1" runat="server">
    <div id="noteContent">
        <h2>Notes</h2>
        <asp:TextBox ID="Note" runat="server" TextMode="multiline" Wrap="true" /><br />
        <asp:Label ID="CharLimit" runat="server" style="font-size: 8px; margin: 0 0 0 15px;">(120 characters max)</asp:Label>
        <p><strong>Days: </strong></p>
        <asp:CheckBoxList ID="radBtns" runat="server" AutoPostBack="false" CellPadding="0" CellSpacing="0"
         RepeatDirection="Horizontal" RepeatColumns="4">
            <asp:ListItem>Mon</asp:ListItem>
            <asp:ListItem>Tue</asp:ListItem>
            <asp:ListItem>Wed</asp:ListItem>
            <asp:ListItem>Thu</asp:ListItem>
            <asp:ListItem>Fri</asp:ListItem>
            <asp:ListItem>Sat</asp:ListItem>
            <asp:ListItem>Sun</asp:ListItem>
        </asp:CheckBoxList>
        <asp:Button ID="Ok" runat="server" CssClass="tbox" OnClick="Ok_OnClick" Text="Save" />
    <script language="javascript" type="text/javascript">
    <asp:Literal ID="notesCloser" runat="server" />
    <asp:Literal ID="errorMsg" runat="server" />
    </script>
    </div>
    </form>
    <script src="Js/firebug.js" type="text/javascript"></script>
</body>
</html>
