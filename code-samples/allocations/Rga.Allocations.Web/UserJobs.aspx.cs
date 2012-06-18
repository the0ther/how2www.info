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
using System.Data.SqlClient;
using RgaData = RGA.Allocations.Data;
using System.Diagnostics;

namespace Allocations
{
    public partial class UserJobs : System.Web.UI.Page
    {
        private int _UserId;
        private int _ClientId;
        private string _Group = string.Empty;
        private int _LoginUserId;
        private int _WeekNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            int userId = 0;
            int clientId = 0;
            int loginUserId = 0;
            int weekNumber = 0;

            if (Request.QueryString["UserId"] != null)
                Int32.TryParse(Request.QueryString["UserId"], out userId);
            if (Request.QueryString["ClientId"] != null)
                Int32.TryParse(Request.QueryString["ClientId"], out clientId);
            if (Request.QueryString["LoginId"] != null)
                Int32.TryParse(Request.QueryString["LoginId"], out loginUserId);
            if (Request.QueryString["WeekNumber"] != null)
                Int32.TryParse(Request.QueryString["WeekNumber"], out weekNumber);

            this.UserId = userId;
            this.ClientId = clientId;
            this.LoginUserId = loginUserId;
            this.WeekNumber = weekNumber;

            if (TestGroupMembership("A3Coordinator"))
                _Group = "A3Coordinator";
            if (TestGroupMembership("A2Administrator"))
                _Group = "A2Administrator";
            if (TestGroupMembership("A8Producer"))
                _Group = "A8Producer";
            if (TestGroupMembership("A5ExecProducer"))
                _Group = "A5ExecProducer";

            if (!Page.IsPostBack)
            {
                BindJobList();
                EmployeeName.Text = GetEmpName(_UserId);
            }
        }

        private int LoginUserId
        {
            set { _LoginUserId = value; }
        }

        private int UserId
        {
            set { _UserId = value; }
        }

        private int ClientId
        {
            set { _ClientId = value; }
        }

        private int WeekNumber
        {
            set { _WeekNumber = value; }
        }

        private void BindJobList()
        {
            using (SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                cnxn.Open();
                SqlCommand cmd = new SqlCommand("ALOC_ManageProjectTeamsJobList", cnxn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@client_id", _ClientId);
                cmd.Parameters.AddWithValue("@user_id", _UserId);
                cmd.Parameters.AddWithValue("@login_id", _LoginUserId);
                cmd.Parameters.AddWithValue("@user_type", _Group);
                cmd.Parameters.AddWithValue("@week_number", _WeekNumber);
                SqlDataReader reader = cmd.ExecuteReader();
                repJobList.DataSource = reader;
                repJobList.DataBind();
                if (cnxn.State == ConnectionState.Open)
                    cnxn.Close();
            }
        }

        protected void Save_OnClick(object sender, EventArgs e)
        {
            //update assignments info
            foreach (RepeaterItem repItem in repJobList.Items)
            {
                //get the JobId
                int job_id = 0;
                HiddenField hid = (HiddenField)repItem.FindControl("hidJobId");
                job_id = Int32.Parse(hid.Value);
                //see if the image is checked/not-checked
                bool assigned = false;
                if (ToAssign.Value.IndexOf(job_id.ToString()) > -1)
                    assigned = true;

                //fire the stored proc
                if (assigned)
                {
                    RgaData.ManageAssignments.Assign(job_id, _UserId);
                }
                else
                {
                    if (!RgaData.ManageAssignments.HasAllocations(job_id, _UserId))
                    {
                        RgaData.ManageAssignments.Unassign(job_id, _UserId, string.Empty);
                    }
                    else if (ToMove.Value.IndexOf(job_id.ToString()) > -1)
                    {
                        RgaData.ManageAssignments.Unassign(job_id, _UserId, "move");
                    }
                    else if (ToDelete.Value.IndexOf(job_id.ToString()) > -1)
                    {
                        RgaData.ManageAssignments.Unassign(job_id, _UserId, "delete");
                    }
                }
            }
            CloseScript.Text = @"
                        var popupiframe = window.parent.document.getElementById(""popupiframe"");
                        popupiframe.src = """";
                        var popuppanel = window.parent.document.getElementById(""popupanel"");
                        popuppanel.style.display = 'none'; 
		                var rebind = window.parent.document.getElementById('rebind');
		                rebind.value = 'true';
                        var form = window.parent.document.getElementById('form1');
                        form.submit();";
        }

        protected void ImgBtn_OnCommand(object sender, CommandEventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            Debug.WriteLine("client id: " + btn.ClientID);
            int jobId = Convert.ToInt32(e.CommandArgument);
            if (sender != null)
            {
                if (btn.ImageUrl == "~/Img/icon_add_off.gif")
                {
                    ToAssign.Value += jobId + ",";
                    ToUnassign.Value.Replace(jobId.ToString() + ",", string.Empty);

                    btn.ImageUrl = "~/Img/icon_add_on.gif";
                    if (ToMove.Value.IndexOf(jobId.ToString()) > -1) // < 0 && ToDelete.Value.IndexOf(jobId.ToString()) < 0)
                        ToMove.Value.Replace(jobId.ToString() + ",", string.Empty);
                    if (ToDelete.Value.IndexOf(jobId.ToString()) > -1)
                        ToDelete.Value.Replace(jobId.ToString() + ",", string.Empty);
                }
                else
                {
                    if (RgaData.ManageAssignments.HasAllocations(jobId, _UserId))
                    {
                        //show the Resolve popup...
                        ResolveScript.Text = @"			
                                var p = new Popup2({
		                            pWidth: 325, pHeight: 150, iWidth: 305, iHeight: 140, src: 'ResolveTeamAllocations.html?userid=" + _UserId + "&jobid=" + jobId + "&img=" + btn.ClientID + "'";

                        ResolveScript.Text += @"});
	                            p.open();";
                    }
                    else
                    {
                        ResolveScript.Text = string.Empty;
                        ToUnassign.Value += jobId + ",";
                        ToAssign.Value.Replace(jobId.ToString() + ",", string.Empty);
                        btn.ImageUrl = "~/Img/icon_add_off.gif";
                    }

                }
            }
        }

        private string GetEmpName(int user_id)
        {
            string retval = string.Empty;
            using (SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                cnxn.Open();
                SqlCommand cmd = new SqlCommand("SELECT FirstName + ' ' + LastName AS FullName FROM AllocableUsers WHERE UserId=@user_id", cnxn);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    retval = reader["FullName"].ToString();
                if (cnxn.State == ConnectionState.Open)
                    cnxn.Close();
            }
            return retval;
        }

        protected void ClearAll_OnClick(object sender, EventArgs e)
        {
            foreach (RepeaterItem repItem in repJobList.Items)
            {
                ImageButton btn = (ImageButton)repItem.FindControl("ImgBtn");
                btn.ImageUrl = "~/Img/icon_add_off.gif";
            }
        }

        protected void SelectAll_OnClick(object sender, EventArgs e)
        {
            foreach (RepeaterItem repItem in repJobList.Items)
            {
                ImageButton btn = (ImageButton)repItem.FindControl("ImgBtn");
                btn.ImageUrl = "~/Img/icon_add_on.gif";
            }
        }

        public bool TestGroupMembership(string groupcode)
        {
            //TODO: remove this and call the base class' version of this routine
            bool retval = false;
            using (SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                cnxn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT m.MemberID 
                                                 FROM dbo.GroupMembers m INNER JOIN 
                                                 dbo.Groups g ON m.GroupID = g.GroupID 
                                                WHERE (m.MemberType=0) AND 
                                                (m.MemberName = @login_user_id) AND 
                                                (g.Code = @group_code)", cnxn);
                cmd.Parameters.AddWithValue("@login_user_id", _LoginUserId);
                cmd.Parameters.AddWithValue("@group_code", groupcode);
                SqlDataReader rr = cmd.ExecuteReader();
                if (rr.Read())
                {
                    retval = true;
                }
                else
                {
                    if (!rr.IsClosed)
                        rr.Close();
                    cmd = new SqlCommand(@" SELECT m.MemberID FROM dbo.GroupMembers m INNER JOIN 
                                            dbo.Groups g ON m.GroupID = g.GroupID 
                                            WHERE (m.MemberType=1)AND(m.MemberName = @dept_id) 
                                            AND (g.Code = @group_code)", cnxn);
                    cmd.Parameters.AddWithValue("@dept_id", _LoginUserId);
                    cmd.Parameters.AddWithValue("@group_code", groupcode);
                    rr = cmd.ExecuteReader();
                    if (rr.Read())
                        retval = true;
                }
                if (cnxn.State == ConnectionState.Open)
                    cnxn.Close();
            }
            return retval;
        }	
    }
}