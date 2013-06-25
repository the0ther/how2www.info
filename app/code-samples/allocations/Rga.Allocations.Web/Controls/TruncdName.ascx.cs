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
using RgaWeb = RGA.Allocations.Web;
using RgaData = RGA.Allocations.Data;
using System.Diagnostics;

namespace Allocations
{
    public partial class Controls_TruncdName : System.Web.UI.UserControl
    {
        private int m_UserId;
        private string m_Title;
        private int m_WeekNumber;
        private string m_FirstName;
        private string m_LastName;
        private int m_Limit;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            NameLink.NavigateUrl = "~/EmployeeAllocation.aspx?EmpId=" + UserId + "&Week=" + WeekNumber;
            if (FirstName != "TBD")
            {
                NameLink.Text = FirstName + " " + LastName;
                NameLink.ToolTip = FirstName + " " + LastName + " / " + Title;
                NameAndTitle.ToolTip = FirstName + " " + LastName + " / " + Title;
            }
            else
            {
                NameLink.Text = FirstName;
                NameLink.ToolTip = FirstName;
                NameAndTitle.ToolTip = FirstName;
            }
            //check the length of name + title if it exceeds limit then trunc the thing...
            int len = (FirstName + " " + LastName + " / " + Title).Length;
            if (len > Limit)
            {
                int shortLen = (FirstName + " " + LastName).Length;
                NameAndTitle.Text = " / " + (FirstName + " " + LastName + Title).Substring((FirstName + LastName).Length+1, Limit - shortLen - 3) + "...";
            }
            else
            {
                if (Title != string.Empty)
                    NameAndTitle.Text = " / " + Title;
                else
                    NameAndTitle.Text = "&nbsp;";
            }
            
        }

        public int UserId
        {
            get { return m_UserId; }
            set { m_UserId = value; }
        }

        public int WeekNumber
        {
            get { return m_WeekNumber; }
            set { m_WeekNumber = value; }
        }

        public string FirstName
        {
            get { return m_FirstName; }
            set { m_FirstName = value; }
        }

        public string LastName
        {
            get { return m_LastName; }
            set { m_LastName = value; }
        }

        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        public int Limit
        {
            get { return m_Limit; }
            set { m_Limit = value; }
        }
    }
}