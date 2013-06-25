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
using RgaWeb = RGA.Allocations.Web;

namespace Allocations
{
    public partial class ProjectAllocation : simpPage
    {
        private delegate bool IsInGroup(ref DBHelper db, string str);
        private delegate void DelSelectWeek();
        protected int _UserId;
        private DateTime _StartDate;
        private int _StartWeek;
        protected int _ClientId;
        protected int[] _Weeks;
        protected DateTime[] _WeeksStartingDates;
        private DataSet _DataSet;
        protected RgaWeb.SecurityType _UserType = RGA.Allocations.Web.SecurityType.EditNone;
        protected int _JobId;
        protected DateTime _JobStartDate;
        protected DateTime _JobEndDate;
        protected string _ResourceType;
        protected int _Region;
        private int numEmps = 0;

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            //note: this is only here to support the page reloading after the Add Jobs panel is closed.
            if (rebind.Value == "true")
            {
                rebind.Value = "false";
                ClientsDrop.SelectedValue = _ClientId.ToString();
                SetFilterValues();
                BindGrid();
            }
            Trace.Write("number of employees: " + numEmps);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsInGroup groupTest = null;
            DelSelectWeek delSelectWeek = new DelSelectWeek(this.ctlWeekPicker_SelectedIndexChanged);
            ctlWeekPicker.UpdateCurrentWeek = delSelectWeek;

            if (Request.Url.Host.ToLower() == "localhost")
            {
                //base.login_user_id = _UserId = 40123; //user
                //base.login_user_id = _UserId = 40110; //exec producer -- skip
                //base.login_user_id = _UserId = 272; //producer -- elyse
                //base.login_user_id = _UserId = 205; //coordinator
                base.login_user_id = _UserId = 144; //administrator -- randy

                theader.Visible = false;
                aheader.Visible = false;
                levaNav.Visible = false;
                tfooter.Visible = false;
                levbNav.Visible = false;
                groupTest = TestGroupMembership;
            }
            else
            {
                base.Page_Load("/newallocations/ProjectAllocation.aspx");
                _UserId = base.login_user_id;
                groupTest = base.testmembership;
            }


            DBHelper db = new DBHelper();
            if (groupTest.Invoke(ref db, "A3Coordinator"))
                _UserType = RgaWeb.SecurityType.EditAll;
            if (groupTest.Invoke(ref db, "A2Administrator"))
                _UserType = RgaWeb.SecurityType.EditAll;
            if (groupTest.Invoke(ref db, "A8Producer"))
                _UserType = RgaWeb.SecurityType.EditUpcoming;
            if (groupTest.Invoke(ref db, "A5ExecProducer"))
                _UserType = RgaWeb.SecurityType.EditUpcoming;

            if (!Page.IsPostBack)
            {
                LoadClients();
                LoadJobs(_ClientId);
                LoadRegions();

                // retrieve current week and date 
                _StartWeek = RgaData.Utility.CurrectWeek();
                _StartDate = RgaData.Utility.CurrectWeekStartDate(_StartWeek);

                // setup week picker
                ctlWeekPicker.CurrentWeek = _StartWeek;
                ctlWeekPicker.CurrectWeekStartDate = _StartDate;

                //handle incoming date parameter as well
                if (Request.QueryString["Week"] != null)
                {
                    int week = -1;
                    Int32.TryParse(Request.QueryString["Week"], out week);
                    if (week != -1)
                    {
                        _StartWeek = week;
                        _StartDate = RgaData.Utility.CurrectWeekStartDate(_StartWeek);
                        ctlWeekPicker.CurrentWeek = _StartWeek;
                        ctlWeekPicker.CurrectWeekStartDate = _StartDate;
                    }
                }
                //if the qs has a project id, load up the page with that project
                if (Request.QueryString["ProjectId"] != null)
                {
                    int proj = Convert.ToInt32(Request.QueryString["ProjectId"]);
                    int client = RgaData.Utility.GetClientIdForJob(proj);
                    AllOrAssigned.SelectedIndex = 0;
                    LoadClients();
                    ClientsDrop.SelectedValue = client.ToString();
                    LoadJobs(client);
                    JobsDrop.SelectedValue = proj.ToString();
                    SetFilterValues();
                    BindGrid();
                }
                SetManageLinkVisibility();
            }
            else
            {
                // set start week
                _StartWeek = ctlWeekPicker.CurrentWeek;
                _ClientId = Convert.ToInt32(ClientsDrop.SelectedValue);
            }
        }

        private void LoadClients()
        {
            ClientsDrop.DataSource = RgaData.Client.Clients(AllOrAssigned.SelectedValue, _UserId);
            ClientsDrop.DataValueField = "ClientId";
            ClientsDrop.DataTextField = "Name";
            ClientsDrop.DataBind();

            ListItem li = new ListItem("Select a Client", "0");
            ClientsDrop.Items.Insert(0, li);
            _ClientId = 0;
        }

        private void LoadJobs(int client)
        {
            if (AllOrAssigned.SelectedValue == "ASSIGNED")
                JobsDrop.DataSource = RgaData.Job.JobsAssignedTo(client, _UserId);
            else
                JobsDrop.DataSource = RgaData.Job.AllJobs(client);
            JobsDrop.DataValueField = "JobId";
            JobsDrop.DataTextField = "Name";
            JobsDrop.DataBind();

            ListItem li = new ListItem("Select a Project", "0");
            JobsDrop.Items.Insert(0, li);
        }

        private void LoadRegions()
        {
            DBHelper db = new DBHelper();
            SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
            base.fillRegionDD(ref db, ref cnxn, ref RegionsDrop);
        }

        protected void repDepts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView parentRow = e.Item.DataItem as DataRowView;
            if (parentRow != null)
            {
                Repeater empsRepeater = e.Item.FindControl("repEmps") as Repeater;
                if (empsRepeater != null)
                {
                    int deptId = Convert.ToInt32(parentRow.Row["DeptId"]);
                    DataView childView = new DataView();
                    childView.Table = _DataSet.Tables["Emps"];
                    childView.RowFilter = "DeptId = '" + deptId.ToString() + "'";
                    empsRepeater.DataSource = childView;
                    empsRepeater.DataBind();
                }
            }
            numEmps++;
        }

        protected void ClientsDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetManageLinkVisibility();
            LoadJobs(_ClientId);
            repDepts.DataSource = null;
            repDepts.DataBind();
        }

        protected void JobsDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetManageLinkVisibility();
            SetFilterValues();
            BindGrid();
        }

        protected void RegionsDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        private void BindGrid()
        {
            _StartWeek = ctlWeekPicker.CurrentWeek;
            _StartDate = ctlWeekPicker.CurrectWeekStartDate;
            _Weeks = new int[] { _StartWeek, _StartWeek + 1, _StartWeek + 2, _StartWeek + 3 };
            _WeeksStartingDates = new DateTime[] { _StartDate, _StartDate.AddDays(7), _StartDate.AddDays(14), _StartDate.AddDays(21) };

            ltrWeekOne.Text = _WeeksStartingDates[0].ToString("MM/dd/yy");
            ltrWeekTwo.Text = _WeeksStartingDates[1].ToString("MM/dd/yy");
            ltrWeekThree.Text = _WeeksStartingDates[2].ToString("MM/dd/yy");
            ltrWeekFour.Text = _WeeksStartingDates[3].ToString("MM/dd/yy");

            _DataSet = RgaData.ProjectGrid.ProjectAllocationsGrid(_ClientId, _StartWeek, 
                    Convert.ToInt32(JobsDrop.SelectedValue), radResourceOptions.SelectedValue,
                    _Region);

            repDepts.DataSource = _DataSet.Tables["Depts"];
            repDepts.DataBind();
            DateTime start = RgaData.Utility.GetJobStartDate(_JobId);
            DateTime end = RgaData.Utility.GetJobEndDate(_JobId);
            ProjDate.Text = "Project Date Range: " + start.ToString("MM/dd/yyyy");
            if (end != DateTime.MinValue)
                ProjDate.Text += " - " + end.ToString("MM/dd/yyyy");

        }

        protected void ctlWeekPicker_SelectedIndexChanged()
        {
            SetFilterValues();
            BindGrid();
        }

        protected void AllOrAssigned_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadClients();
            LoadJobs(_ClientId);
            repDepts.DataSource = null;
            repDepts.DataBind();
            SetManageLinkVisibility();
        }

        protected void radResourceOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        private void SetFilterValues()
        {
            _ClientId = Convert.ToInt32(ClientsDrop.SelectedValue);
            _JobId = Convert.ToInt32(JobsDrop.SelectedValue);
            _StartWeek = ctlWeekPicker.CurrentWeek;
            _StartDate = ctlWeekPicker.CurrectWeekStartDate;
            _ResourceType = radResourceOptions.SelectedValue;
            _JobStartDate = RgaData.Utility.GetJobStartDate(_JobId);
            _JobEndDate = RgaData.Utility.GetJobEndDate(_JobId);
            _Region = Convert.ToInt32(RegionsDrop.SelectedValue);
        }

        private void SetManageLinkVisibility()
        {
            ManageLink.Visible = false;
            ManageLink.Text = string.Empty;
            /*
            if (JobsDrop.SelectedIndex <= 0)
            {
                Pipe.Visible = ManageLink.Visible = false;
            }
            else if (_UserType == RgaWeb.SecurityType.EditUpcoming)
            {
                if (AllOrAssigned.SelectedValue == "ASSIGNED")
                    Pipe.Visible = ManageLink.Visible = true;
                else
                {
                    using (SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                    {
                        cnxn.Open();
                        SqlCommand cmd = new SqlCommand("ALOC_IsAssigned", cnxn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@user_id", _UserId);
                        cmd.Parameters.AddWithValue("@client_id", _ClientId);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                            Pipe.Visible = ManageLink.Visible = Convert.ToBoolean(reader[0].ToString());
                        if (cnxn.State == ConnectionState.Open)
                            cnxn.Close();
                    }
                }
            }
            else
            {
                Pipe.Visible = ManageLink.Visible = false;
            }
            if (ManageLink.Visible)
            {
                ManageLink.Text = @"<a href=""javascript:;"" id=""asdf"" onclick=""openManagePanel();"" class=""red"">Manage</a>";
            }
            else
            {
                ManageLink.Text = string.Empty;
            }
            */
        }

        //TODO: search all files for Width and Height and eliminate style from the ASP tags
        private bool TestGroupMembership(ref DBHelper notUsed, string groupcode)
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
                cmd.Parameters.AddWithValue("@login_user_id", base.login_user_id);
                cmd.Parameters.AddWithValue("@group_code", groupcode);
                SqlDataReader rr = cmd.ExecuteReader();
                if (rr.Read()) //TODO: do I need to actually check the value???
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
                    cmd.Parameters.AddWithValue("@dept_id", base.login_dept_id);
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