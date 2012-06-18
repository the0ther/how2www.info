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
    public partial class EmployeeAllocation : simpPage
    {
        private delegate bool IsInGroup(ref DBHelper db, string str);
        private delegate void DelSelectWeek();
        protected int _UserId;
        private DateTime _StartDate;
        private int _StartWeek;
        protected int _DeptId;
        protected int[] _Weeks;
        protected DateTime[] _WeeksStartingDates;
        private DataSet _DataSet;
        protected RgaWeb.SecurityType _UserType = RGA.Allocations.Web.SecurityType.EditNone;
        protected int _EmpId;
        protected DateTime _JobStartDate;
        protected DateTime _JobEndDate;
        protected string _ResourceType;
        protected bool _IsTbdUser;
        private int numClients = 0;

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
                base.Page_Load("/newallocations/EmployeeAllocation.aspx");
                _UserId = base.login_user_id;
                groupTest = base.testmembership;
            }

            DBHelper db = new DBHelper();
            if (groupTest.Invoke(ref db, "A3Coordinator"))
                _UserType = RgaWeb.SecurityType.EditAll;
            if (groupTest.Invoke(ref db, "A2Administrator"))
                _UserType = RgaWeb.SecurityType.EditAll;

            if (!Page.IsPostBack)
            {
                LoadDepts();

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
                //if a user-id is passed in on the qs, set dropdowns and databind
                if (Request.QueryString["EmpId"] != null)
                {
                    int user = Convert.ToInt32(Request.QueryString["EmpId"]);
                    int dept = RgaData.Utility.GetDeptIdForUser(user);
                    if (dept != -1)
                    {
                        DeptsDrop.SelectedValue = dept.ToString();
                        LoadEmps(dept);
                        EmpsDrop.SelectedValue = user.ToString();
                        SetFilterValues();
                        BindGrid();
                    }
                }
            }
            else
            {
                // set start week
                _StartWeek = ctlWeekPicker.CurrentWeek;
                _DeptId = Convert.ToInt32(DeptsDrop.SelectedValue);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Trace.Write("count of clients: " + numClients);
        }

        private void LoadDepts()
        {
            DeptsDrop.DataSource = RgaData.Department.Departments();
            DeptsDrop.DataValueField = "DeptId";
            DeptsDrop.DataTextField = "Name";
            DeptsDrop.DataBind();

            ListItem li = new ListItem("Select a Department", "0");
            DeptsDrop.Items.Insert(0, li);
            _DeptId = 0;
        }

        private void LoadEmps(int dept)
        {
            EmpsDrop.DataSource = RgaData.Employee.Employees(dept);
            EmpsDrop.DataValueField = "UserId";
            EmpsDrop.DataTextField = "FullName";
            EmpsDrop.DataBind();

            ListItem li = new ListItem("Select an Employee", "0");
            EmpsDrop.Items.Insert(0, li);
        }

        protected void EmpsDrop_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem item in EmpsDrop.Items)
            {
                if (item.Text.IndexOf("&nbsp;") > -1)
                    item.Text = item.Text.Replace("&nbsp;", string.Empty);
            }
        }

        protected void repClients_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //bind the child repeater
            DataRowView parentRow = e.Item.DataItem as DataRowView;
            
            if (parentRow != null)
            {
                Repeater empsRepeater = e.Item.FindControl("repJobs") as Repeater;
                if (empsRepeater != null)
                {
                    int deptId = Convert.ToInt32(parentRow.Row["ClientId"]);
                    DataView childView = new DataView();
                    childView.Table = _DataSet.Tables["Jobs"];
                    childView.RowFilter = "ClientId2 = " + deptId.ToString();
                    empsRepeater.DataSource = childView;
                    empsRepeater.DataBind();
                }
            }
            //bind the header 
            if (e.Item.ItemType == ListItemType.Header)
            {
                float avail = 0;
                float total = 0;

                Label lbl = e.Item.FindControl("ltrWeek1Avail") as Label;
                avail = Convert.ToSingle(_DataSet.Tables["Availability"].Rows[0][0]);
                total = Convert.ToSingle(RgaWeb.Utility.ConvertMinutesToHours(
                    Convert.ToInt32(_DataSet.Tables["Availability"].Rows[0][4])));
                lbl.Text = avail.ToString() + "/" + total.ToString();
                MarkOverOrUnder(lbl, avail, total);

                lbl = e.Item.FindControl("ltrWeek2Avail") as Label;
                avail = Convert.ToSingle(_DataSet.Tables["Availability"].Rows[0][1]);
                total = Convert.ToSingle(RgaWeb.Utility.ConvertMinutesToHours(
                    Convert.ToInt32(_DataSet.Tables["Availability"].Rows[0][5])));
                lbl.Text = avail.ToString() + "/" + total.ToString();
                MarkOverOrUnder(lbl, avail, total);

                lbl = e.Item.FindControl("ltrWeek3Avail") as Label;
                avail = Convert.ToSingle(_DataSet.Tables["Availability"].Rows[0][2]);
                total = Convert.ToSingle(RgaWeb.Utility.ConvertMinutesToHours(
                    Convert.ToInt32(_DataSet.Tables["Availability"].Rows[0][6])));
                lbl.Text = avail.ToString() + "/" + total.ToString();
                MarkOverOrUnder(lbl, avail, total);

                lbl = e.Item.FindControl("ltrWeek4Avail") as Label;
                avail = Convert.ToSingle(_DataSet.Tables["Availability"].Rows[0][3]);
                total = Convert.ToSingle(RgaWeb.Utility.ConvertMinutesToHours(
                    Convert.ToInt32(_DataSet.Tables["Availability"].Rows[0][7])));
                lbl.Text = avail.ToString() + "/" + total.ToString();
                MarkOverOrUnder(lbl, avail, total);
            }
            else
            {
                //count clients in here, the block above is for the header only...
                numClients++;
            }
        }

        protected void DeptsDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmps(_DeptId);
            repClients.DataSource = null;
            repClients.DataBind();
        }

        protected void EmpsDrop_SelectedIndexChanged(object sender, EventArgs e)
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

            if (_IsTbdUser)
                _DataSet = RgaData.EmpGrid.TbdEmpAllocsGrid(_EmpId, _StartWeek);
            else
                _DataSet = RgaData.EmpGrid.EmpAllocationsGrid(_EmpId, _StartWeek);


            if (_DataSet.Tables["Clients"].Rows.Count > 0)
            {
                repClients.Visible = true;
                repClients.DataSource = _DataSet.Tables["Clients"];
                repClients.DataBind();
            }
            else
            {
                repClients.Visible = false;
            }
            //set the title label for this employee
            Title.Text = "Current Title: " + RgaData.Utility.GetTitleForEmp(_EmpId);
        }

        protected void ctlWeekPicker_SelectedIndexChanged()
        {
            SetFilterValues();
            BindGrid();
        }

        private void SetFilterValues()
        {
            _DeptId = Convert.ToInt32(DeptsDrop.SelectedValue);
            _EmpId = Convert.ToInt32(EmpsDrop.SelectedValue);
            _StartWeek = ctlWeekPicker.CurrentWeek;
            _StartDate = ctlWeekPicker.CurrectWeekStartDate;
            if (EmpsDrop.SelectedItem.Text.Trim().ToUpper() == "TBD")
                _IsTbdUser = true;
            else
                _IsTbdUser = false;
        }

        private void MarkOverOrUnder(Label lbl, float avail, float total)
        {
            if (avail > total)
                lbl.CssClass = "under";
            else if (total > avail)
                lbl.CssClass = "over";
        }

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