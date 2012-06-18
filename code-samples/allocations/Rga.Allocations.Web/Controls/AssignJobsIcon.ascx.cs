using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using RgaWeb=RGA.Allocations.Web;

namespace Allocations
{
    public partial class Controls_AssignJobsIcon : System.Web.UI.UserControl
    {
        protected string _realPerson = string.Empty;
        protected int _currentWeek = 0;
        protected string _loginUserId = string.Empty;
        protected int _thisUserId = 0;
        protected int _clientId = -1;
        protected RgaWeb.SecurityType _userType = RgaWeb.SecurityType.EditNone;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Debug.WriteLine("the client id in Page_Load is: " + _clientId);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (_realPerson.ToUpper() != "Y" || _userType != RgaWeb.SecurityType.EditUpcoming)
                Icon.Visible = false;
            else
                Icon.Visible = true;
        }

        public string RealPerson
        {
            set { _realPerson = value; }
        }

        public int CurrentWeek
        {
            set { _currentWeek = value; }
        }

        public string LoginUserid
        {
            set { _loginUserId = value; }
        }

        public int UserId
        {
            set { _thisUserId = value; }
        }

        public int ClientId2
        {
            set {
                Debug.WriteLine("setting client id property to: " + value);
                _clientId = value; 
            }
        }

        public RgaWeb.SecurityType UserType
        {
            set { _userType = value; }
        }

    }
}