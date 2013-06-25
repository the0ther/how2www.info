<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WeekPicker.ascx.cs" Inherits="Allocations.Controls_WeekPicker" EnableViewState="true" %>
<!-- this input is here to fix a bug where hitting Enter when in a textbox submits the form -->
<asp:ImageButton ID="DoNothing" OnClientClick="return false;" ImageUrl="~/Img/c.gif" Width="0px" Height="0px" runat="server" />
<asp:ImageButton ID="PrevFourWeeks" runat="server" OnClick="PrevFourWeeks_Click" Visible="false" ImageUrl="~/Img/dbl_arrow_left_off.gif" />
<asp:ImageButton ID="lnkbPrevious" runat="server" OnClick="lnkbPrevious_Click" ImageUrl="~/Img/arrow_left_off.jpg"></asp:ImageButton>
<asp:Literal ID="ltrStartDate" runat="server" Text="00/00" />
<asp:Literal ID="ltrEndDate" runat="server" Text="00/00" />
<asp:ImageButton ID="lnkbNext" runat="server" OnClick="lnkbNext_Click" ImageUrl="~/Img/arrow_right_off.jpg"></asp:ImageButton>
<asp:ImageButton ID="NextFourWeeks" runat="server" OnClick="NextFourWeeks_Click" Visible="false" ImageUrl="~/Img/dbl_arrow_right_off.gif" />