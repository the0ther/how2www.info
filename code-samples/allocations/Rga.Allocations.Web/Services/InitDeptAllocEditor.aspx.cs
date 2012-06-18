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
using System.Data.SqlClient;

namespace Allocations
{
    public partial class Services_InitDeptAllocEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int jobId = -1;
            int userId = -1;
            int weekNumber = -1;
            float hrsAllocated = 0.0f;

            Response.Expires = 1;
            Response.CacheControl = "no-cache";

            if (Request.QueryString["JobId"] != null)
                Int32.TryParse(Request.QueryString["JobId"], out jobId);
            if (Request.QueryString["UserId"] != null)
                Int32.TryParse(Request.QueryString["UserId"], out userId);
            if (Request.QueryString["WeekNumber"] != null)
                Int32.TryParse(Request.QueryString["WeekNumber"], out weekNumber);


            JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output);
            writer.WriteStartArray();

            using (SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                cnxn.Open();
                SqlCommand cmd = new SqlCommand("SELECT AnyMins FROM Allocations WHERE JobId=@job_id AND UserId=@user_id AND WeekNumber=@week_number", cnxn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@job_id", jobId);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@week_number", weekNumber);
                SqlDataReader reader = cmd.ExecuteReader();
                writer.WriteStartObject();
                writer.WriteMember("hours");
                if (reader.Read())
                {
                    hrsAllocated = RGA.Allocations.Web.Utility.ConvertMinutesToHours(Convert.ToInt32(reader["AnyMins"]));
                    writer.WriteNumber(hrsAllocated);
                }
                else
	            {
                    writer.WriteNumber(0);
	            }
                reader.Close();
                cmd = new SqlCommand("SELECT FirstName, LastName, DeptId FROM AllocableUsers WHERE UserId=@user_id", cnxn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    writer.WriteMember("firstname");
                    writer.WriteString(reader["FirstName"].ToString());
                    writer.WriteMember("lastname");
                    writer.WriteString(reader["LastName"].ToString());
                }
                else
                {
                    writer.WriteMember("firstname");
                    writer.WriteString(string.Empty);
                    writer.WriteMember("lastname");
                    writer.WriteString(string.Empty);
                }
                writer.WriteEndObject();
                if (cnxn.State == ConnectionState.Open)
                    cnxn.Close();
            }
            writer.WriteEndArray();
        }
    }
}