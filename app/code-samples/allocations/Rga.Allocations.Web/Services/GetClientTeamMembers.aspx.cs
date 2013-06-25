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
using RgaWeb = RGA.Allocations.Web;
using Jayrock.Json;

public partial class Services_GetClientTeamMembers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int team = -1;
        Response.Expires = 1;
        Response.CacheControl = "no-cache";

        if (Request.QueryString["TeamId"] != null)
        {
            Int32.TryParse(Request.QueryString["TeamId"], out team);
            if (team > -1)
            {
                DataTable dt = RgaData.TeamGrid.ClientTeamMembers(team);

                using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
                {
                    writer.WriteStartArray();
                    foreach (DataRow row in dt.Rows)
                    {
                        writer.WriteStartObject();
                        writer.WriteMember("TeamMemberId");
                        writer.WriteNumber(row["TeamMemberId"].ToString());
                        writer.WriteMember("UserId");
                        writer.WriteNumber(row["UserId"].ToString());
                        writer.WriteMember("FullName");
                        writer.WriteString(row["FullName"].ToString());
                        writer.WriteMember("ShortName");
                        writer.WriteString(row["ShortName"].ToString());
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
            }
        }
    }
}
