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

public partial class Services_UpdateAssignment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int jobId=0, userId=0, retval=0;
        string action = string.Empty;
        string existing = string.Empty;

        if (Request.QueryString["JobId"] != null)
            Int32.TryParse(Request.QueryString["JobId"], out jobId);
        if (Request.QueryString["UserId"] != null)
            Int32.TryParse(Request.QueryString["UserId"], out userId);
        if (Request.QueryString["action"] != null)
            action = Request.QueryString["action"];
        if (Request.QueryString["existing"] != null)
            existing = Request.QueryString["existing"];

        if (jobId > 0 && userId > 0)
        {
            if (action == "assign")
                RgaData.ManageAssignments.Assign(jobId, userId);
            else if (action == "unassign")
            {
                //returns 0 if no time billed, 1 if time billed, 99 if user has future allocations
                if (existing != string.Empty)
                {
                    retval = RgaData.ManageAssignments.Unassign(jobId, userId, existing);
                }
                else if (RgaData.ManageAssignments.HasAllocations(jobId, userId))
                {
                    retval = 99;
                }
                else
                {
                    retval = RgaData.ManageAssignments.Unassign(jobId, userId, string.Empty);
                }
                Response.Write(retval);
            }
        }
    }
}