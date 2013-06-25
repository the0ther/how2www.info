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
using System.Text;
using RgaData = RGA.Allocations.Data;
using RgaWeb = RGA.Allocations.Web;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Allocations
{
    public partial class TeamAllocation : Allocations.simpPage
    {
        protected RgaWeb.SecurityType _secType = RgaWeb.SecurityType.EditNone;
        private int _UserId;
        protected int _ClientId = 0;
        private int _StartWeek = 0;
        private int _DepartmentId = 0;
        private int _ClientGroupId = 0;
        private string _ResourceType;
        private DateTime _StartDate;
        delegate void DelSelectWeek();
        private DataSet _DataSet;
        protected int[] _Weeks;
        protected DateTime[] _WeeksStartingDates;
        protected int _Region;
        delegate void DelModifyTeam();
        public int resCount = 0;

        //this delegate is to make testing for group membership work both locally and on the dev server
        //if this is run locally it will point at the TestGroupMembership function in Utils, otherwise 
        //this will point to the function testmembership in the simpPage class
        private delegate bool IsInGroup(ref DBHelper db, string str);

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Debug.WriteLine("entering LoadComplete clientid: " + _ClientId);
            //note: this is only here to support the page reloading after the Add Jobs panel is closed.
            if (rebind.Value == "true")
            {
                rebind.Value = "false";
                dropClient.SelectedValue = _ClientId.ToString();
                SetFilterValues();
                string prevGroupId = dropClientTeam.SelectedValue;
                LoadClientGroups(_ClientId);
                if (dropClientTeam.Items.FindByValue(prevGroupId) != null)
                    dropClientTeam.SelectedValue = prevGroupId;
                BindTeamGrid();
            }
            Trace.Write("number of employees: " + resCount);
            Debug.WriteLine("leaving LoadComplete clientid: " + _ClientId);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Debug.WriteLine("togglesRecord is: " + togglesRecord.Value);
            Debug.WriteLine("entering Load client id: " + _ClientId);
            IsInGroup groupTest = null;
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
                base.Page_Load("/newallocations/TeamAllocation.aspx");
                _UserId = base.login_user_id;
                groupTest = base.testmembership;
            }


            // determine if the user's group and set a variable to use when printing out allocations
            DBHelper db = new DBHelper();
            if (groupTest.Invoke(ref db, "A3Coordinator"))
                _secType = RgaWeb.SecurityType.EditAll;
            if (groupTest.Invoke(ref db, "A2Administrator"))
                _secType = RgaWeb.SecurityType.EditAll;
            if (groupTest.Invoke(ref db, "A8Producer"))
                _secType = RgaWeb.SecurityType.EditUpcoming;
            if (groupTest.Invoke(ref db, "A5ExecProducer"))
                _secType = RgaWeb.SecurityType.EditUpcoming;

            if (!Page.IsPostBack)
            {
                LoadClients();
                LoadDepartments(_ClientId);
                LoadClientGroups(0);
                LoadRegions();

                // retrieve current week and date 
                _StartWeek = RgaData.Utility.CurrectWeek();
                _StartDate = RgaData.Utility.CurrectWeekStartDate(_StartWeek);

                // setup week picker
                ctlWeekPicker.CurrentWeek = _StartWeek;
                ctlWeekPicker.CurrectWeekStartDate = _StartDate;

                if (_ClientId > 0)
                    LoadClientGroups(_ClientId);
            }
            else
            {
                // set start week
                _StartWeek = ctlWeekPicker.CurrentWeek;
                _ClientId = Convert.ToInt32(dropClient.SelectedValue);


            }

            // set up delegate to rebind the grid when week selection is changed
            DelSelectWeek delSelectWeek = new DelSelectWeek(this.ctlWeekPicker_SelectedIndexChanged);
            ctlWeekPicker.UpdateCurrentWeek = delSelectWeek;
            
            //Debug.WriteLine("togglesRecord is: " + togglesRecord.Value);
            Debug.WriteLine("exiting LoadPage clientid: " + _ClientId);
        }

        private void BindTeamGrid()
        {
            // bind header row
            _Weeks = new int[] { _StartWeek, _StartWeek + 1, _StartWeek + 2, _StartWeek + 3 };
            _WeeksStartingDates = new DateTime[] { _StartDate, _StartDate.AddDays(7), _StartDate.AddDays(14), _StartDate.AddDays(21) };

            ltrWeekOne.Text = _WeeksStartingDates[0].ToString("MM/dd/yy");
            ltrWeekTwo.Text = _WeeksStartingDates[1].ToString("MM/dd/yy");
            ltrWeekThree.Text = _WeeksStartingDates[2].ToString("MM/dd/yy");
            ltrWeekFour.Text = _WeeksStartingDates[3].ToString("MM/dd/yy");

            _DataSet = RgaData.TeamGrid.TeamAllocationsGrid(_ClientId, _StartWeek, 
                                        _DepartmentId, _ClientGroupId, _ResourceType,
                                        _Region);

            // bind  user grid
            repResources.DataSource = _DataSet.Tables["User"];
            repResources.DataBind();
        }

        protected void dropClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            LoadClientGroups(_ClientId);
            LoadDepartments(_ClientId);
            BindTeamGrid();
            SetManageLinkVisibility();
        }

        protected void ctlWeekPicker_SelectedIndexChanged()
        {
            SetFilterValues();
            BindTeamGrid();
        }

        protected void radClientOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadClients();
            SetManageLinkVisibility();
        }

        private void LoadClients()
        {
            dropClient.DataSource = RgaData.Client.Clients(radClientOptions.SelectedValue, _UserId);
            dropClient.DataValueField = "ClientId";
            dropClient.DataTextField = "Name";
            dropClient.DataBind();

            ListItem li = new ListItem("Select a Client", "0");
            dropClient.Items.Insert(0, li);
        }

        private void LoadDepartments(int clientId)
        {
            dropDepartment.DataSource = RgaData.Department.Deparments(clientId);
            dropDepartment.DataValueField = "DepartmentId";
            dropDepartment.DataTextField = "Name";
            dropDepartment.DataBind();

            ListItem li = new ListItem("All Departments", "0");
            dropDepartment.Items.Insert(0, li);
        }

        private void LoadClientGroups(int clientId)
        {
            if (clientId != 0)
            {
                dropClientTeam.DataSource = RgaData.ClientGroup.ClientGroups(clientId);
                dropClientTeam.DataValueField = "ClientGroupId";
                dropClientTeam.DataTextField = "Name";
                dropClientTeam.DataBind();
            }

            ListItem li = new ListItem("All Client Groups", "0");
            if (!dropClientTeam.Items.Contains(li))
                dropClientTeam.Items.Insert(0, li);
        }

        private void LoadRegions()
        {
            DBHelper db = new DBHelper();
            SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
            base.fillRegionDD(ref db, ref cnxn, ref RegionsDrop);
        }

        protected void dropDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindTeamGrid();
        }

        protected void dropClientTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindTeamGrid();
        }

        protected void radResourceOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindTeamGrid();
        }

        protected void RegionsDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindTeamGrid();
        }

        private void SetFilterValues()
        {
            _ClientId = Convert.ToInt32(dropClient.SelectedValue);
            _DepartmentId = Convert.ToInt32(dropDepartment.SelectedValue);
            _ClientGroupId = Convert.ToInt32(dropClientTeam.SelectedValue);
            _StartWeek = ctlWeekPicker.CurrentWeek;
            _StartDate = ctlWeekPicker.CurrectWeekStartDate;
            _ResourceType = radResourceOptions.SelectedValue;
            _Region = Convert.ToInt32(RegionsDrop.SelectedValue);
        }

        protected void repResources_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView parentRow = e.Item.DataItem as DataRowView;
            if (parentRow != null)
            {
                Repeater jobsRepeater = e.Item.FindControl("repJobs") as Repeater;
                if (jobsRepeater != null)
                {
                    int userId = Convert.ToInt32(parentRow.Row["UserId"]);
                    DataView childView = new DataView();
                    childView.Table = _DataSet.Tables["Job"];
                    childView.RowFilter = "UserId = '" + userId.ToString() + "'";
                    jobsRepeater.DataSource = childView;
                    jobsRepeater.DataBind();
                }
            }
            resCount++;
        }

        private void SetManageLinkVisibility()
        {

            if (_secType == RgaWeb.SecurityType.EditAll)
            {
                ManageLink.Visible = true;
            }
            else if (_secType == RgaWeb.SecurityType.EditUpcoming)
            {
                if (radClientOptions.SelectedValue == "ASSIGNED")
                    ManageLink.Visible = true;
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
                            ManageLink.Visible = Convert.ToBoolean(reader[0].ToString());
                        if (cnxn.State == ConnectionState.Open)
                            cnxn.Close();
                    }
                }
            }
            else
            {
                ManageLink.Visible = false;
            }
            if (ManageLink.Visible)
            {
                ManageLink.Text = @"<a href=""javascript:;"" id=""asdf"" onclick=""openManagePanel();"" class=""red"">Manage Client Groups</a>";
            }
            else
            {
                ManageLink.Text = string.Empty;
            }
        }

        /// <summary>
        /// this method is here as a workaround to the problems running the site from localhost.
        /// a simliar method exists in the page base class (simpPage) but we cannot call base.Page_Load() 
        /// when running the site from localhost. base.Page_Load() must be called to make the "real" 
        /// testmembership routine work, so when you run locally this method is called, when running 
        /// on the dev or live server, the base-class testmembership function is called. this is made 
        /// possible by using a delegate in the code which calls these group-membership functions.
        /// </summary>
        /// <param name="notUsed"></param>
        /// <param name="groupcode"></param>
        /// <returns></returns>
        public bool TestGroupMembership(ref DBHelper notUsed, string groupcode)
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