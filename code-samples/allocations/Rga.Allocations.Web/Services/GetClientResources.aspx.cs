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
using System.Collections.Generic;

public partial class Services_GetClientResources : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Expires = 1;
        Response.CacheControl = "no-cache";

        //dept, client, team
        int deptid = -1;
        int clientid = -1;
        int teamid = -1;
        if (Request.QueryString["DeptId"] != null)
            Int32.TryParse(Request.QueryString["DeptId"], out deptid);
        if (Request.QueryString["ClientId"] != null)
            Int32.TryParse(Request.QueryString["ClientId"], out clientid);
        if (Request.QueryString["TeamId"] != null)
            Int32.TryParse(Request.QueryString["TeamId"], out teamid);
        if (deptid > -1 && clientid > -1 && teamid > -1)
        {
            List<RgaData.ManageTeamResourceRow> emps = (List<RgaData.ManageTeamResourceRow>)RgaData.ManageTeamDataSource.GetEmployees(deptid, clientid, teamid);
            using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
            {
                writer.WriteStartArray();
                foreach (RgaData.ManageTeamResourceRow row in emps)
                {
                    //write out a json object
                    writer.WriteStartObject();
                    writer.WriteMember("EmpId");
                    writer.WriteNumber(row.EmpId);
                    writer.WriteMember("FullName");
                    writer.WriteString(row.FullName);
                    writer.WriteMember("Title");
                    writer.WriteString(row.Title);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }
        //Response.Flush();
        //Response.End();
    }
}
