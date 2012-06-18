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

namespace Allocations
{
    public partial class UpdateAllocation : System.Web.UI.Page
    {
        //TODO: try this out as a PageMethod
        protected void Page_Load(object sender, EventArgs e)
        {
            int doNotAssign = 0;
            Int32.TryParse(Request.QueryString["doNotAssign"], out doNotAssign);
            int[] retval = null;
            try
            {
                //http://localhost:4439/Rga.Allocations.Web/UpdateAllocation.aspx?allocationid=-1&mins=5400&jobid=4045&employeeid=796&week=323
                retval = RgaData.Allocation.Update(Convert.ToInt32(Request.QueryString["employeeid"]),
                                           Convert.ToInt32(Request.QueryString["jobid"]),
                                           Convert.ToInt32(Request.QueryString["week"]),
                                           Convert.ToInt32(Request.QueryString["allocationid"]),
                                           Convert.ToInt32(Request.QueryString["mins"]),
                                           doNotAssign
                                          );
                using (JsonWriter writer = RgaWeb.Utility.CreateJsonWriter(Response.Output))
                {
                    writer.WriteStartArray();
                    writer.WriteStartObject();
                    writer.WriteMember("AllocId");
                    writer.WriteNumber(retval[0]);
                    writer.WriteMember("NoteLength");
                    writer.WriteNumber(retval[1]);
                    writer.WriteEndObject();
                    writer.WriteEndArray();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}