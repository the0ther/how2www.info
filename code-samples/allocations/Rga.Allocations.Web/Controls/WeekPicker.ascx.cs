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

namespace Allocations
{
    public partial class Controls_WeekPicker : System.Web.UI.UserControl
    {
        private int _CurrentWeek;
        private DateTime _CurrectWeekStartDate;
        private System.Delegate _delUpdateCurrentWeek;
        private int _WeekSpan = 3;
        private bool _ShowEndDate = true;
        private bool _ShowDoubleArrows = false;

        public int CurrentWeek
        {
            set { _CurrentWeek = value; }
            get
            {
                if (_CurrentWeek != 0)
                    return _CurrentWeek;
                else if (ViewState["CurrentWeek"] != null)
                    return Convert.ToInt32(ViewState["CurrentWeek"]);
                else
                    return 0;
            }
        }

        public DateTime CurrectWeekStartDate
        {
            set { _CurrectWeekStartDate = value; }
            get
            {
                if (_CurrectWeekStartDate != null && _CurrectWeekStartDate != DateTime.MinValue)
                    return _CurrectWeekStartDate;
                else if (ViewState["CurrectWeekStartDate"] != null)
                    return Convert.ToDateTime(ViewState["CurrectWeekStartDate"]);
                else
                    return DateTime.MinValue;
            }
        }

        public System.Delegate UpdateCurrentWeek
        {
            set { _delUpdateCurrentWeek = value; }
        }

        public int WeekSpan
        {
            set { _WeekSpan = value; }
        }

        public bool ShowEndDate
        {
            set { _ShowEndDate = value; }
        }

        public bool ShowDoubleArrows
        {
            set { _ShowDoubleArrows = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetViewState();
                lnkbPrevious.Attributes.Add("onmouseover", "this.src='img/arrow_left_hover.jpg'");
                lnkbPrevious.Attributes.Add("onmouseout", "this.src='img/arrow_left_off.jpg'");
                lnkbNext.Attributes.Add("onmouseover", "this.src='img/arrow_right_hover.jpg'");
                lnkbNext.Attributes.Add("onmouseout", "this.src='img/arrow_right_off.jpg'");
                PrevFourWeeks.Attributes.Add("onmouseover", "this.src='img/dbl_arrow_left_hover.gif'");
                PrevFourWeeks.Attributes.Add("onmouseout", "this.src='img/dbl_arrow_left_off.gif'");
                NextFourWeeks.Attributes.Add("onmouseover", "this.src='img/dbl_arrow_right_hover.gif'");
                NextFourWeeks.Attributes.Add("onmouseout", "this.src='img/dbl_arrow_right_off.gif'");
            }
            else
            {
                _CurrentWeek = Convert.ToInt32(ViewState["CurrentWeek"]);
                _CurrectWeekStartDate = Convert.ToDateTime(ViewState["CurrectWeekStartDate"]);
            }

            SetControls();

        }
        protected void lnkbPrevious_Click(object sender, ImageClickEventArgs e)
        {
            _CurrentWeek--;
            _CurrectWeekStartDate = _CurrectWeekStartDate.Add(TimeSpan.FromDays(-7));

            SetControls();
            SetViewState();
            _delUpdateCurrentWeek.DynamicInvoke();
        }

        protected void lnkbNext_Click(object sender, ImageClickEventArgs e)
        {
            _CurrentWeek++;
            _CurrectWeekStartDate = _CurrectWeekStartDate.Add(TimeSpan.FromDays(7));

            SetControls();
            SetViewState();
            _delUpdateCurrentWeek.DynamicInvoke();
        }

        protected void PrevFourWeeks_Click(object sender, EventArgs e)
        {
            _CurrentWeek -= 4;
            _CurrectWeekStartDate = _CurrectWeekStartDate.Subtract(TimeSpan.FromDays(28));
            SetControls();
            SetViewState();
            _delUpdateCurrentWeek.DynamicInvoke();
        }

        protected void NextFourWeeks_Click(object sender, EventArgs e)
        {
            _CurrentWeek += 4;
            _CurrectWeekStartDate = _CurrectWeekStartDate.Add(TimeSpan.FromDays(28));
            SetControls();
            SetViewState();
            _delUpdateCurrentWeek.DynamicInvoke();
        }

        private void SetControls()
        {
            ltrStartDate.Text = _CurrectWeekStartDate.ToString("MM/dd/yy");
            if (_ShowEndDate)
                ltrEndDate.Text = "&nbsp;-&nbsp;" + _CurrectWeekStartDate.Add(TimeSpan.FromDays(_WeekSpan * 7)).ToString("MM/dd/yy");
            else
                ltrEndDate.Text = "";

            if (_ShowDoubleArrows)
            {
                PrevFourWeeks.Visible = true;
                NextFourWeeks.Visible = true;
            }
        }



        private void SetViewState()
        {
            ViewState["CurrentWeek"] = _CurrentWeek;
            ViewState["CurrectWeekStartDate"] = _CurrectWeekStartDate;
        }

    }
}