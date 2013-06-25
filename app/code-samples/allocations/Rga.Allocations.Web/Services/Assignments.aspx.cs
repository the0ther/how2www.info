using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using RgaData = RGA.Allocations.Data;
using RgaWeb = RGA.Allocations.Web;
using Jayrock.Json;

public partial class Services_Assignments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int _jobId = 0;
        DataTable dt = null;

        Response.Expires = 1;
        Response.CacheControl = "no-cache";

        if (Request.QueryString["JobId"] != null)
            Int32.TryParse(Request.QueryString["JobId"], out _jobId);

        if (_jobId > 0)
        {
            dt = RgaData.ManageAssignments.GetAssigned(_jobId);
            using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
            {
                writer.WriteStartArray();
                foreach (DataRow row in dt.Rows)
                {
                    writer.WriteStartObject();
                    writer.WriteMember("id");
                    writer.WriteNumber(row["UserId"].ToString());
                    writer.WriteMember("fullname");
                    writer.WriteString(row["FullName"].ToString());
                    writer.WriteMember("deptname");
                    writer.WriteString(row["DeptName"].ToString());
                    writer.WriteMember("timebilled");
                    writer.WriteNumber(row["TimeBilled"].ToString());
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }

    }
}
