using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using RgaData = RGA.Allocations.Data;
using RgaWeb = RGA.Allocations.Web;
using System.Diagnostics;
using System.Data;

namespace Allocations
{
    public partial class RegionAllocation : simpPage
    {
        private delegate void DelSelectWeek();
        protected RgaWeb.SecurityType _secType = RgaWeb.SecurityType.EditNone;
        private delegate bool IsInGroup(ref DBHelper db, string str);
        private int _UserId;
        private int _StartWeek = 0;
        private DateTime _StartDate;
        private DataSet _DataSet;
        protected int[] _Weeks;
        protected DateTime[] _WeeksStartingDates;
        private string _ResourceType;
        private int _Region = 0;
        private int _DepartmentId = 0;
        protected int _empCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
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
                LoadRegions();
                LoadDepartments();

                // retrieve current week and date 
                _StartWeek = RgaData.Utility.CurrectWeek();
                _StartDate = RgaData.Utility.CurrectWeekStartDate(_StartWeek);

                // setup week picker
                ctlWeekPicker.CurrentWeek = _StartWeek;
                ctlWeekPicker.CurrectWeekStartDate = _StartDate;
            }
            else
            {
                // set start week
                _StartWeek = ctlWeekPicker.CurrentWeek;
            }

            // set up delegate to rebind the grid when week selection is changed
            DelSelectWeek delSelectWeek = new DelSelectWeek(this.ctlWeekPicker_SelectedIndexChanged);
            ctlWeekPicker.UpdateCurrentWeek = delSelectWeek;
        }

        protected void Regions_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            LoadDepartments();
            BindGrid();
        }

        protected void ResourceOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        protected void Department_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        protected void Resources_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView parentRow = e.Item.DataItem as DataRowView;
            if (parentRow != null)
            {
                Repeater jobsRepeater = e.Item.FindControl("Jobs") as Repeater;
                if (jobsRepeater != null)
                {
                    int userId = Convert.ToInt32(parentRow.Row["UserId"]);
                    DataView childView = new DataView();
                    childView.Table = _DataSet.Tables["Jobs"];
                    childView.RowFilter = "UserId = '" + userId.ToString() + "'";
                    Debug.WriteLine("found the inner repeater. userid: " + userId);
                    jobsRepeater.DataSource = childView;
                    jobsRepeater.DataBind();
                }
            }
            _empCount++;
        }

        protected void ctlWeekPicker_SelectedIndexChanged()
        {
            SetFilterValues();
            BindGrid();
        }

        private void SetFilterValues()
        {
            _DepartmentId = Convert.ToInt32(Department.SelectedValue);
            _StartWeek = ctlWeekPicker.CurrentWeek;
            _StartDate = ctlWeekPicker.CurrectWeekStartDate;
            _ResourceType = ResourceOptions.SelectedValue;
            _Region = Convert.ToInt32(Regions.SelectedValue);
        }

        private void BindGrid()
        {
            _Weeks = new int[] { _StartWeek, _StartWeek + 1, _StartWeek + 2, _StartWeek + 3 };
            _WeeksStartingDates = new DateTime[] { _StartDate, _StartDate.AddDays(7), _StartDate.AddDays(14), _StartDate.AddDays(21) };
            ltrWeekOne.Text = _WeeksStartingDates[0].ToString("MM/dd/yy");
            ltrWeekTwo.Text = _WeeksStartingDates[1].ToString("MM/dd/yy");
            ltrWeekThree.Text = _WeeksStartingDates[2].ToString("MM/dd/yy");
            ltrWeekFour.Text = _WeeksStartingDates[3].ToString("MM/dd/yy");
            _DataSet = RgaData.RegionGrid.RegionAllocationsGrid(_Region, _StartWeek, _DepartmentId, _ResourceType);
            Resources.DataSource = _DataSet.Tables["User"];
            Resources.DataBind();

        }

        private void LoadRegions()
        {
            DBHelper db = new DBHelper();
            SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
            base.fillRegionDD(ref db, ref cnxn, ref Regions);
            Regions.Items.RemoveAt(0);
            Regions.Items.Insert(0, new ListItem("Select a Region", "-1"));
        }

        private void LoadDepartments()
        {
            Department.DataSource = RgaData.Department.Departments();
            Department.DataValueField = "DeptId";
            Department.DataTextField = "Name";
            Department.DataBind();

            ListItem li = new ListItem("All Departments", "0");
            Department.Items.Insert(0, li);
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