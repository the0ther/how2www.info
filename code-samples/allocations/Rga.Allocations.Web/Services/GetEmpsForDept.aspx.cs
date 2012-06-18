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
using Jayrock.Json;
using RgaData = RGA.Allocations.Data;
using RgaWeb = RGA.Allocations.Web;

namespace Allocations
{
    public partial class Services_GetEmpsForDept : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int deptId = 0;

            Response.Expires = 1;
            Response.CacheControl = "no-cache";

            if (Request.QueryString["DeptId"] != null)
                Int32.TryParse(Request.QueryString["DeptId"], out deptId);

            DataTable dt = RgaData.DeptGrid.GetEmpsForDept(deptId, true);
            using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
            {
                writer.WriteStartArray();
                foreach (DataRow row in dt.Rows)
                {
                    writer.WriteStartObject();
                    writer.WriteMember("userid");
                    writer.WriteNumber(row["UserId"].ToString());
                    writer.WriteMember("firstname");
                    writer.WriteString(row["FirstName"].ToString());
                    writer.WriteMember("lastname");
                    writer.WriteString(row["LastName"].ToString());
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }
    }
}