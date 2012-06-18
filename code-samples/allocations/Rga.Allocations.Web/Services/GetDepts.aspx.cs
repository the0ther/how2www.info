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
using Jayrock.Json;
using RgaData = RGA.Allocations.Data;
using RgaWeb = RGA.Allocations.Web;

public partial class Services_GetDepts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Expires = 1;
        Response.CacheControl = "no-cache";

        DataTable dt = RgaData.TeamGrid.GetDepts();
        using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
        {
            writer.WriteStartArray();
            foreach (DataRow row in dt.Rows)
            {
                writer.WriteStartObject();
                writer.WriteMember("DeptId");
                writer.WriteNumber(row["DeptId"].ToString());
                writer.WriteMember("DeptName");
                writer.WriteString(row["Name"].ToString());
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}
