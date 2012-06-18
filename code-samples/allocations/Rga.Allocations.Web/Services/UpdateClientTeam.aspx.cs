using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using RgaData = RGA.Allocations.Data;

public partial class Services_UpdateClientTeam : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int teamid = -1;
        string retval = string.Empty;
        string action = string.Empty;
        int userid = -1;
        int memberid = -1;
        string name = string.Empty;
        string desc = string.Empty;
        int client = -1;

        Response.Expires = 1;
        Response.CacheControl = "no-cache";

        if (Request.QueryString["TeamMemberId"] != null)
            Int32.TryParse(Request.QueryString["TeamMemberId"], out memberid);
        if (Request.QueryString["TeamId"] != null)
            Int32.TryParse(Request.QueryString["TeamId"], out teamid);
        if (Request.QueryString["action"] != null)
            action = Request.QueryString["action"];
        if (Request.QueryString["UserId"] != null)
            Int32.TryParse(Request.QueryString["UserId"], out userid);
        if (Request.QueryString["TeamName"] != null)
            name = Request.QueryString["TeamName"];
        if (Request.QueryString["TeamDesc"] != null)
            desc = Request.QueryString["TeamDesc"];
        if (Request.QueryString["ClientId"] != null)
            Int32.TryParse(Request.QueryString["ClientId"], out client);

        //now call funcs in data layer based on action (remove, add)
        if (memberid > -1 && action == "remove")
            retval = RgaData.TeamGrid.RemoveFromClientTeam(memberid).ToString();
        else if (userid > -1 && teamid > -1 && action == "add")
            retval = RgaData.TeamGrid.AddToClientTeam(teamid, userid).ToString();
        else if (teamid > -1 && action == "delete")
            RgaData.TeamGrid.DeleteClientTeam(teamid);
        else if (name != string.Empty && client > -1 && action == "create")
            retval = RgaData.TeamGrid.CreateNewClientTeam(name, client, desc).ToString();
        else if (name != string.Empty && teamid > -1 && action == "edit")
            RgaData.TeamGrid.EditClientTeam(teamid, name, desc);
        else if (teamid > -1 && action == "delete")
            RgaData.TeamGrid.DeleteClientTeam(teamid);
        Response.Write(retval);
    }
}
