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
using System.Text;
using RgaData = RGA.Allocations.Data;
using RgaWeb = RGA.Allocations.Web;
using System.Diagnostics;

namespace Allocations
{
    public partial class DeptAllocation : simpPage
    {
        protected RgaWeb.SecurityType _secType = RgaWeb.SecurityType.EditNone;
        private delegate bool IsInGroup(ref DBHelper db, string str);
        private delegate void DelSelectWeek();
        protected int _UserId;
        protected int _DeptId;
        protected string _resType;
        protected string _Title;
        protected int _WeekNumber;
        protected DateTime _StartDate;
        protected int _ClientId;
        private DataSet _Data;
        protected int _Region;
        protected int _resCount;

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
                base.Page_Load("/newallocations/DeptAllocation.aspx");
                _UserId = base.login_user_id;
                groupTest = base.testmembership;
            }

            // determine if the user's group and set a variable to use when printing out allocations
            DBHelper db = new DBHelper();
            if (groupTest.Invoke(ref db, "A3Coordinator"))
                _secType = RgaWeb.SecurityType.EditAll;
            if (groupTest.Invoke(ref db, "A2Administrator"))
                _secType = RgaWeb.SecurityType.EditAll;

            if (!Page.IsPostBack)
            {
                // retrieve current week and date 
                _WeekNumber = RgaData.Utility.CurrectWeek();
                _StartDate = RgaData.Utility.CurrectWeekStartDate(_WeekNumber);

                // setup week picker
                WeekPicker1.CurrentWeek = _WeekNumber;
                WeekPicker1.CurrectWeekStartDate = _StartDate;

                WeekNumber.Value = _WeekNumber.ToString();
            }
            WeekPicker1.WeekSpan = 1;
            WeekPicker1.ShowEndDate = false;
            DelSelectWeek delSelectWeek = new DelSelectWeek(this.WeekPicker1_SelectedIndexChanged);
            WeekPicker1.UpdateCurrentWeek = delSelectWeek;
        }

        private void WeekPicker1_SelectedIndexChanged()
        {
            WeekNumber.Value = WeekPicker1.CurrentWeek.ToString();
            SetFilterValues();
            BindGrid();
        }

        protected void Titles_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        private void SetFilterValues()
        {
            _DeptId = Convert.ToInt32(DeptsDrop.SelectedValue);
            _resType = radResourceOptions.SelectedValue;
            _Title = TitlesDrop.SelectedValue;
            _WeekNumber = WeekPicker1.CurrentWeek;
            _ClientId = Convert.ToInt32(ClientsDrop.SelectedValue);
            if (RegionsDrop.SelectedValue != string.Empty)
                _Region = Convert.ToInt32(RegionsDrop.SelectedValue);
            else
                _Region = 0;
        }

        private void BindGrid()
        {
            //TODO: bind grid gets called twice wtf?
            //get the data
            _Data = RGA.Allocations.Data.DeptGrid.DeptAllocationsGrid(_DeptId, _WeekNumber, _resType, _Title, _ClientId, _Region);
            MainGrid.Rows.Clear();
            if (_Data.Tables["Emps"].Rows.Count > 1 && _Data.Tables["Jobs"].Rows.Count > 0)
            {
                ArrayList uniqueUserIds = MakeEmpsHeader();
                Trace.Write("number of employees: " + uniqueUserIds.Count);
                MakeJobRows(uniqueUserIds);
                Trace.Write("number of jobs: " + _Data.Tables["Jobs"].Rows.Count);
                FillGrid();
            }
            else
            {
                Trace.Write("number of employees: 0");
                Trace.Write("number of jobs: 0");
            }
            _Data.Clear();
        }

        private void FillGrid()
        {
            if (MainGrid.Rows.Count > 0)
            {
                TableRow empsRow = MainGrid.Rows[0];
                _resCount = empsRow.Cells.Count - 1;
                for (int ii = 1; ii < empsRow.Cells.Count; ii++)
                {
                    //get the employee cell from header row, so we can look at the hiddenfield for this user-id
                    TableHeaderCell th = (TableHeaderCell)empsRow.Cells[ii];
                    if (th.Controls.Count > 0)
                    {
                        string empId = th.Attributes["emp"];
                        if (empId != null && empId != string.Empty)
                        {
                            //now that we've got the hiddenfield, go through each cell in this job-row
                            for (int jj = 2; jj < MainGrid.Rows.Count; jj++)
                            {
                                TableRow jobRow = MainGrid.Rows[jj];
                                TableCell td = null;
                                Literal lit = null;
                                if (!(jobRow.Cells[0].Controls[0] is Image))
                                {
                                    //this is a job row
                                    string jobId = jobRow.Cells[0].Attributes["job"];

                                    DataRow[] rows = _Data.Tables["Emps"].Select("UserId=" + empId + " AND JobId=" + jobId + " AND WeekNum=" + _WeekNumber);


                                    td = jobRow.Cells[ii];
                                    Label val = new Label();
                                    if (rows != null && rows.Length > 0)
                                    {
                                        Debug.Assert(rows.Length == 1);
                                        float hrs = Convert.ToSingle(RgaWeb.Utility.ConvertMinutesToHours(Convert.ToInt32(rows[0]["AnyMins"])).ToString());
                                        if (td.Controls.Count >= 1)
                                        {
                                            lit = (Literal)td.Controls[0];
                                            //no fucking clue why this is even necessary, but IT IS
                                            int s = lit.Text.IndexOf("id=\"allocNote_");
                                            int e = lit.Text.IndexOf("_true");
                                            if (e < 0)
                                                e = lit.Text.IndexOf("_false");
                                            string allocId = lit.Text.Substring(s + 14, e - (s + 14));
                                            if (Convert.ToInt32(allocId) != Convert.ToInt32(rows[0].ItemArray[9]))
                                            {
                                                lit.Text = lit.Text.Replace(allocId.ToString(), rows[0].ItemArray[9].ToString());
                                                if (hrs != 0)
                                                {
                                                    int noteLen = rows[0].ItemArray[10].ToString().Length;
                                                    if (noteLen > 0)
                                                        lit.Text = lit.Text.Replace("Img/c.gif", "Img/icon_note_on.gif");
                                                    else
                                                        lit.Text = lit.Text.Replace("Img/c.gif", "Img/icon_note_off.gif");
                                                }
                                            }
                                        }
                                        if (hrs == 0 && td.Controls.Count == 1)
                                        {

                                            lit.Text = lit.Text.Replace("Img/icon_note_off.gif", "Img/c.gif");
                                            lit.Text = lit.Text.Replace("Img/icon_note_on.gif", "Img/c.gif");

                                            int start = lit.Text.IndexOf("showNotes");
                                            int end = lit.Text.IndexOf("return false");
                                            if (start > 0)
                                                lit.Text = lit.Text.Substring(0, start) + lit.Text.Substring(end);
                                        }
                                        else
                                        {
                                            Debug.WriteLine("hrs is: " + hrs);
                                        }
                                        //now add another literal for the value of this allocation
                                        lit = new Literal();
                                        lit.Text = "<span id=\"val_" + empId + "_" + jobId + "\" class=\"hrs\">" +
                                                        hrs.ToString() + "</span>";
                                        td.Controls.Add(lit);
                                        Literal br = new Literal();
                                        br.Text = "<br/>";
                                        td.Controls.Add(br);
                                        HtmlImage img = new HtmlImage();
                                        img.Src = "img/c.gif";
                                        img.Width = 50;
                                        img.Height = 0;
                                        img.Attributes.Add("class", "invis");
                                        td.Controls.Add(img);
                                    }
                                    else
                                    {
                                        //try clearing out the event handler
                                        lit = (Literal)td.Controls[0];
                                        lit.Text = lit.Text.Replace("Img/icon_note_off.gif", "Img/c.gif");
                                        lit.Text = lit.Text.Replace("Img/icon_note_on.gif", "Img/c.gif");
                                        int start = lit.Text.IndexOf("showNotes");
                                        int end = lit.Text.IndexOf("return false");
                                        if (start > 0)
                                            lit.Text = lit.Text.Substring(0, start) + lit.Text.Substring(end);


                                        lit = new Literal();
                                        lit.Text = "<span id=\"val_" + empId + "_" + jobId + "\" width=\"25px\" class=\"hrs\">&nbsp;</span>";
                                        td.Controls.Add(lit);
                                        Literal br = new Literal();
                                        br.Text = "<br/>";
                                        td.Controls.Add(br);
                                        HtmlImage img = new HtmlImage();
                                        img.Src = "img/c.gif";
                                        img.Width = 50;
                                        img.Height = 0;
                                        img.Attributes.Add("class", "invis");
                                        td.Controls.Add(img);
                                    }
                                    td.ToolTip = GridCellTooltip(jobId, empId);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("here");
            }
        }

        private string GridCellTooltip(string jobId, string userId)
        {
            string retval = string.Empty;
            DataRow[] empRec = _Data.Tables["Emps"].Select("UserId=" + userId);
            if (empRec.Length > 0 && empRec[0]["LastName"].ToString() != string.Empty)
                retval = empRec[0]["FirstName"].ToString() + " " + empRec[0]["LastName"].ToString();
            else if (empRec[0]["LastName"].ToString() == string.Empty)
                retval = empRec[0]["FirstName"].ToString();
            DataRow[] jobRecord = _Data.Tables["Jobs"].Select("JobId=" + jobId);
            if (jobRecord.Length > 0 && retval.Length > 0)
                retval += " — " + jobRecord[0]["JobName"].ToString();
            else
                retval = jobRecord[0]["JobName"].ToString();
            return HttpUtility.HtmlAttributeEncode(retval);
        }

        private TableRow MakeClientRow(int clientId, string name, ArrayList userIds)
        {
            TableRow retval = new TableRow();
            retval.CssClass = "main-row";

            TableCell td = new TableCell();
            td.CssClass = "info-name";
            //add an image for the row toggle
            Image img = new Image();
            img.CssClass = "toggle-row";
            img.ImageUrl = "img/icn_open.gif";
            img.ID = "toggleImage_" + clientId.ToString();
            td.Controls.Add(img);
            Label lbl = new Label();
            lbl.Text = name;
            td.Controls.Add(lbl);

            retval.Cells.Add(td);
            //now add a blank cell for each employee
            for (int ii = 0; ii < userIds.Count; ii++)
            {
                td = new TableCell();
                retval.Cells.Add(td);
            }

            return retval;
        }

        private TableRow MakeJobRow(int jobId, string jobName, ArrayList users, string jobCode, object start, object end)
        {
            TableRow retval = new TableRow();
            retval.Cells.Clear();
            retval.CssClass = "rowalt";
            
            TableCell td = new TableCell();
            td.Controls.Clear();

            td.CssClass = "info-name";

            td.Attributes.Add("job", jobId.ToString());
            //chop the job name if it's more than 46 chars long
            string name = jobName;
            if (name.Length > 40)
                name = name.Substring(0, 40) + "...";
            HyperLink link = new HyperLink();
            link.Text = name;
            link.CssClass = "name";
            link.NavigateUrl = "ProjectAllocation.aspx?ProjectId=" + jobId.ToString() + "&Week=" + _WeekNumber;

            string sd = string.Empty;
            string ed = string.Empty;
            if (start != DBNull.Value)
                sd = Convert.ToDateTime(start).ToString("MM/dd/yyyy");
            if (end != DBNull.Value)
                ed = Convert.ToDateTime(end).ToString("MM/dd/yyyy");
            link.ToolTip = jobCode + " - " + jobName + " (" + sd + " - " + ed + ")";
            td.Controls.Add(link);

            retval.Cells.Add(td);
            //now add a blank cell for each employee
            string OffImg = "Img/icon_note_off.gif";
            string OnImg = "Img/icon_note_on.gif";
            foreach (int userId in users)
            {
                string hideImg = "position:relative; float:right; margin: 0; padding: 0; cursor: pointer;'";
                td = new TableCell();
                td.VerticalAlign = VerticalAlign.Top;
                td.ID = "t_" + userId + "_" + jobId;
                Literal lit = new Literal();
                int allocId = 0;
                string offOrOn = OffImg;
                DataRow[] rows = _Data.Tables["Emps"].Select("JobId=" + jobId + " AND UserId=" + userId);
                if (rows.Length > 0)
                {
                    allocId = Convert.ToInt32(rows[0]["AllocId"]);
                    hideImg = "style='cursor: pointer;'";
                    foreach (DataRow row in rows)
                        if (row["Note"] != DBNull.Value && row["Note"].ToString() != string.Empty)
                            offOrOn = OnImg;
                }
                string enabled = _secType == RGA.Allocations.Web.SecurityType.EditAll ? "true" : "false";
                lit.Text = String.Format(@"<span class=""img""><img border=""0"" class=""img"" src=""{2}"" id=""allocNote_{0}_{1}"" {3} /></span>",
                                new object[] { allocId, enabled, offOrOn, hideImg });
                td.Controls.Add(lit);

                retval.Cells.Add(td);
            }
            return retval;
        }

        private void MakeJobRows(ArrayList users)
        {
            if (users.Count > 0)
            {
                string currentClient = string.Empty;

                //setup a <tr> for each job in the jobs datatable
                TableRow tr = null;
                foreach (DataRow row in _Data.Tables["Jobs"].Rows)
                {
                    if (row["Name"].ToString() != currentClient)
                    {
                        //make a client row
                        currentClient = row["Name"].ToString();
                        tr = MakeClientRow(Convert.ToInt32(row["clientId"]), currentClient, users);
                        //now use that same row to make the first job row for this client
                        MainGrid.Rows.Add(tr);
                        tr = MakeJobRow(Convert.ToInt32(row["JobId"]), row["JobName"].ToString(),
                            users, row["JobCode"].ToString(), row["StartDate"], row["EndDate"]);
                    }
                    else
                    {
                        //make a job row
                        tr = MakeJobRow(Convert.ToInt32(row["JobId"]), row["JobName"].ToString(), 
                             users, row["JobCode"].ToString(), row["StartDate"], row["EndDate"]);
                    }
                    MainGrid.Rows.Add(tr);
                }
            }
            else
            {
                MainGrid.Rows.Clear();
            }
        }

        private ArrayList MakeEmpsHeader()
        {
            ArrayList retval = new ArrayList();
            //MainGrid
            string currentEmp = string.Empty;

            //setup the table header row
            TableHeaderRow tr = new TableHeaderRow();
            tr.CssClass = "header-row";
            //add a blank td for the upper-left corner
            TableHeaderCell th = new TableHeaderCell();
            Image img = new Image();
            img.CssClass = "toggle-all";
            img.ImageUrl = "img/icn_open.gif";
            th.Controls.Add(img);
            Label lbl = new Label();
            lbl.Text = "Jobs";
            th.Controls.Add(lbl);
            th.CssClass = "header-name";
            tr.Cells.Add(th);

            //setup a row to display user's availability info
            TableRow availabilityRow = new TableRow();
            availabilityRow.CssClass = "header-row";
            TableCell td = new TableCell();
            availabilityRow.Cells.Add(td);


            //this sorts the results
            DataRow[] rows = _Data.Tables["Emps"].Select("1=1", "realPerson, FirstName");

            //now make a th cell for each employee
            foreach (DataRow row in rows)
            {
                if (currentEmp != row["FirstName"] + " " + row["LastName"])
                {
                    string fullName = row["FirstName"] + " " + row["LastName"];
                    if (fullName.Length > 7)
                    {
                        fullName = row["FirstName"].ToString().Length >= 5 ? row["FirstName"].ToString().Substring(0,5) : row["FirstName"].ToString();
                        if (row["LastName"].ToString() != "&nbsp;")
                        {
                            fullName += " ";
                            fullName += row["LastName"].ToString().Length >= 5 ? row["LastName"].ToString().Substring(0, 5) : row["LastName"].ToString();
                        }
                    }
                    currentEmp = row["FirstName"] + " " + row["LastName"];
                    th = new TableHeaderCell();
                    th.Attributes.Add("emp", row["UserId"].ToString());
                    HyperLink link = new HyperLink();
                    link.Text = fullName.Trim();
                    link.NavigateUrl = "EmployeeAllocation.aspx?EmpId=" + row["UserId"].ToString() + "&Week=" + _WeekNumber;
                    if (row["Title"].ToString().Length > 0)
                        link.ToolTip = currentEmp + " / " + row["Title"].ToString();
                    else
                        link.ToolTip = currentEmp;
                    if (link.ToolTip.EndsWith("&nbsp;"))
                        link.ToolTip = link.ToolTip.Replace("&nbsp;", string.Empty);
                    link.Attributes.Add("style", "color: #ffffff; text-decoration: underline; font-weight: bold;");
                    th.Controls.Add(link);
                    tr.Cells.Add(th);

                    td = new TableCell();
                    td.VerticalAlign = VerticalAlign.Middle;
                    Literal available = new Literal();
                    string overOrUnder = string.Empty;
                    if (Convert.ToSingle(row["HrsAvailable"]) > RgaWeb.Utility.ConvertMinutesToHours(Convert.ToInt32(row["HrsAllocated"])))
                        overOrUnder = "under ";
                    else if (Convert.ToSingle(row["HrsAvailable"]) < RgaWeb.Utility.ConvertMinutesToHours(Convert.ToInt32(row["HrsAllocated"])))
                        overOrUnder = "over ";
                    available.Text = "<span id=\"avail_" + row["UserId"].ToString() + "\" class=\"" + overOrUnder + "hrs\"" +
                                " title=\"Available / Allocated\">" + 
                               row["HrsAvailable"].ToString() + "/" +
                                RgaWeb.Utility.ConvertMinutesToHours(Convert.ToInt32(row["HrsAllocated"])).ToString() + 
                                "</span>";
                    td.Controls.Add(available);
                    availabilityRow.Cells.Add(td);
                    retval.Add(Convert.ToInt32(row["UserId"]));
                }
            }
            MainGrid.Rows.Add(tr);
            availabilityRow.Cells[0].CssClass = "info-name";
            HtmlImage clear = new HtmlImage();
            clear.Src = "img/c.gif";
            clear.Width = 370;
            clear.Height = 0;
            availabilityRow.Cells[0].Controls.Add(clear);
            //availabilityRow.Cells[0].Controls.Add(new 
            MainGrid.Rows.Add(availabilityRow);
            //int width = 61 * tr.Cells.Count + 330;
            //int width = 51 * tr.Cells.Count + 330;
            int width = 51 * tr.Cells.Count + 300;
            MainGrid.Width = Unit.Pixel(width);
            return retval;
        }

        protected void Clients_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        protected void Depts_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        protected void DeptsDrop_DataBound(object sender, EventArgs e)
        {
            DeptsDrop.Items.Insert(0, new ListItem("Select a Department", "-1"));
        }

        protected void Clients_DataBound(object sender, EventArgs e)
        {
            ClientsDrop.Items.Insert(0, new ListItem("All Clients", "-1"));
        }

        protected void Titles_DataBound(object sender, EventArgs e)
        {
            TitlesDrop.Items.Insert(0, new ListItem("All Titles", ""));
        }

        protected void Regions_DataBound(object sender, EventArgs e)
        {
            RegionsDrop.Items.Insert(0, new ListItem("All Regions", ""));
        }

        protected void Resources_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
        }

        protected void RegionsDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterValues();
            BindGrid();
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