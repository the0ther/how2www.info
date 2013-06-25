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
using RGA.Allocations.Data;
using System.Data.SqlClient;

namespace Allocations
{
    public partial class LockAllocations : Allocations.simpPage
    {
        private delegate bool IsInGroup(ref DBHelper db, string str);
        private IsInGroup groupTest = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Url.Host.ToLower() == "localhost")
            {
                //base.login_user_id = 40123; //user
                //base.login_user_id = 40110; //exec producer -- skip
                //base.login_user_id = 272; //producer -- elyse
                //base.login_user_id = 205; //coordinator
                base.login_user_id = 144; //administrator -- randy
                theader.Visible = false;
                aheader.Visible = false;
                levaNav.Visible = false;
                tfooter.Visible = false;
                levbNav.Visible = false;
                groupTest = this.TestGroupMembership;
            }
            else
            {
                base.Page_Load("/newallocations/LockAllocations.aspx");
                groupTest = base.testmembership;
            }
            DBHelper db = new DBHelper();
            if (!groupTest.Invoke(ref db, "A3Coordinator") && !groupTest.Invoke(ref db, "A2Administrator"))
                Response.Redirect("/permitError.html");

            int week_num = RGA.Allocations.Data.Utility.CurrectWeek() + 1;
            WeekNumber.Value = week_num.ToString();
            Week.Text = RGA.Allocations.Data.Utility.CurrectWeekStartDate(week_num).ToString("MM/dd/yy");
        }

        protected void btn_OnCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "lock")
            {
                ImageButton btn = (ImageButton)sender;
                if (btn.ImageUrl.IndexOf("off") > 0)
                {
                    LocksDS.InsertParameters[0].DefaultValue = WeekNumber.Value;
                    LocksDS.InsertParameters[1].DefaultValue = e.CommandArgument.ToString();
                    LocksDS.Insert();
                }
                else
                {
                    LocksDS.DeleteParameters[0].DefaultValue = WeekNumber.Value;
                    LocksDS.DeleteParameters[1].DefaultValue = e.CommandArgument.ToString();
                    LocksDS.Delete();
                }
                LocksGrid.DataBind();
            }
        }

        protected void Unlock_OnClick(object sender, EventArgs e)
        {
            foreach (GridViewRow row in LocksGrid.Rows)
            {
                ImageButton imgBtn = (ImageButton)row.FindControl("btn");
                LocksDS.DeleteParameters[0].DefaultValue = WeekNumber.Value;
                LocksDS.DeleteParameters[1].DefaultValue = imgBtn.CommandArgument.ToString();
                LocksDS.Delete();
            }
            LocksGrid.DataBind();

        }

        protected void Lock_OnClick(object sender, EventArgs e)
        {
            foreach (GridViewRow row in LocksGrid.Rows)
            {
                ImageButton imgBtn = (ImageButton)row.FindControl("btn");
                LocksDS.InsertParameters[0].DefaultValue = WeekNumber.Value;
                LocksDS.InsertParameters[1].DefaultValue = imgBtn.CommandArgument.ToString();
                LocksDS.Insert();
            }
            LocksGrid.DataBind();
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
        private bool TestGroupMembership(ref DBHelper notUsed, string groupcode)
        {
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