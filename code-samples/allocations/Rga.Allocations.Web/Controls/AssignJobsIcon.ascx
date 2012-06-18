<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssignJobsIcon.ascx.cs" Inherits="Allocations.Controls_AssignJobsIcon" %>
<asp:Panel CssClass="employee-icon" ID="Icon" runat="server">
	<a href="javascript:;"  onclick="showpopup(this.id, <%= _currentWeek %>, <% = _loginUserId %>, <%# _thisUserId %>, <%# _clientId %>);">
		<img src="Img/icn_info.gif" width="6" height="12" alt="Assign Jobs" title="Assign Jobs" />
    </a>
</asp:Panel>