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
//using System.IO;

public partial class Services_GetClientTeams : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int client = -1;
        int teamid = -1;
        DataTable dt = null;

        Response.Expires = 1;
        Response.CacheControl = "no-cache";
        if (Request.QueryString["TeamId"] != null)
            Int32.TryParse(Request.QueryString["TeamId"], out teamid);
        if (Request.QueryString["ClientId"] != null)
            Int32.TryParse(Request.QueryString["ClientId"], out client);
        if (teamid > -1 && client > -1)
        {
            //get info for single team
            dt = RgaData.TeamGrid.ClientTeams(client);
            DataRow[] row = dt.Select("TeamId=" + teamid.ToString());
            if (row != null && row.Length > 0)
            {
                using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
                {
                    writer.WriteStartArray();
                    writer.WriteStartObject();
                    writer.WriteMember("TeamId");
                    writer.WriteNumber(row[0]["TeamId"].ToString());
                    writer.WriteMember("TeamName");
                    writer.WriteString(row[0]["Name"].ToString());
                    writer.WriteMember("Description");
                    writer.WriteString(row[0]["Description"].ToString());
                    writer.WriteEndObject();
                    writer.WriteEndArray();
                }
            }
        }
        else if (client > -1)
        {
            dt = RgaData.TeamGrid.ClientTeams(client);

            using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
            {
                writer.WriteStartArray();
                foreach (DataRow row in dt.Rows)
                {
                    writer.WriteStartObject();
                    writer.WriteMember("TeamId");
                    writer.WriteNumber(row["TeamId"].ToString());
                    writer.WriteMember("TeamName");
                    writer.WriteString(row["Name"].ToString());
                    writer.WriteMember("Description");
                    writer.WriteString(row["Description"].ToString());
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }
    }
}
