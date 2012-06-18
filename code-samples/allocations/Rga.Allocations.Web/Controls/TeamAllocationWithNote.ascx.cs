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
using RgaWeb = RGA.Allocations.Web;
using System.Diagnostics;

namespace Allocations
{
    public partial class Controls_TeamAllocationWithNote : System.Web.UI.UserControl
    {
        //added these fields to support the different behavior of this control on project vs. team views.
        private int _AvailableHours;
        private int _TotalMins;

        //these fields are the "original" fields
        private int _WeekNumber;
        private int _UserId;
        private int _JobId;
        private int _Minutes;
        private int _AllocId = -1;
        private DateTime _Start;
        private DateTime _End;
        private string _Note;
        private RgaWeb.SecurityType _UserType = RGA.Allocations.Web.SecurityType.EditNone;
        private int _ClientId = -1;
        private const string OffImg = "Img/icon_note_off.gif";
        private const string OnImg = "Img/icon_note_on.gif";
        private const string inputTemplate = @"<input type=""text"" class=""txt"" maxlength=""5"" id=""week_{0}_{1}_{2}_{6}_{9}"" value=""{3}"" {8} />";
        private const string imgInputTemplate = @"<input type=""image"" class=""img"" src=""{3}"" id=""allocNote_{0}_{1}"" onclick=""return false;"" {2}/>";
        private const string clearGif = @"<input type=""image"" class=""img"" src=""Img/c.gif"" id=""allocNote_{0}_{1}"" onclick=""return false;"" style=""cursor: default"" />";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string textBox = string.Empty;
            string image = string.Empty;
            string imageUrl = string.Empty;

            if (_Note == string.Empty)
                imageUrl = OffImg;
            else
                imageUrl = OnImg;

            DateTime current = RgaData.Utility.CurrectWeekStartDate(Convert.ToInt32(_WeekNumber));

            //select case based on security type
            switch ((int)_UserType)
            {
                case 0:
                    textBox = String.Format(inputTemplate, new object[] {_WeekNumber,_UserId,_JobId,RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (_Minutes)),
                                                _WeekNumber,_UserId,_Minutes,_JobId, RGA.Allocations.Web.Utility.IsNotBetween(_Start,_End,current),"_" + _AllocId});
                    if (RGA.Allocations.Web.Utility.ConvertMinutesToHours(Convert.ToInt32(_Minutes)) == 0)
                        image = String.Format(clearGif, new object[] { _AllocId, "true" });
                    else
                        image = String.Format(imgInputTemplate, new object[] { _AllocId, "true", "", imageUrl });
                    break;
                case 1: //edit upcoming
                    if (current > _Start.AddDays(1) && DateTime.Now <= current &&
                        !RgaData.Allocation.IsLocked(Convert.ToInt32(_WeekNumber), _ClientId))
                    {
                        textBox = String.Format(inputTemplate, new object[] {_WeekNumber,_UserId,_JobId,RGA.Allocations.Web.Utility.ConvertMinutesToHours( Convert.ToInt32 (_Minutes)),
                                                _WeekNumber,_UserId,_Minutes,_JobId, RGA.Allocations.Web.Utility.IsNotBetween(_Start,_End,current),"_" + _AllocId});
                        if (RGA.Allocations.Web.Utility.ConvertMinutesToHours(Convert.ToInt32(_Minutes)) == 0)
                            image = String.Format(clearGif, new object[] { _AllocId, "true" });
                        else
                            image = String.Format(imgInputTemplate, new object[] { _AllocId, "true", "", imageUrl });
                    }
                    else
                    {
                        textBox = RGA.Allocations.Web.Utility.ConvertMinutesToHours(Convert.ToInt32(_Minutes)).ToString();
                        if (RGA.Allocations.Web.Utility.ConvertMinutesToHours(Convert.ToInt32(_Minutes)) == 0 || _Note.Length == 0)
                            image = String.Format(clearGif, new object[] { _AllocId, "false" });
                        else
                            image = String.Format(imgInputTemplate, new object[] { _AllocId, "false", "", imageUrl });
                    }
                    break;
                case 2: //read-only
                    textBox = RGA.Allocations.Web.Utility.ConvertMinutesToHours(Convert.ToInt32(_Minutes)).ToString();
                    if (RGA.Allocations.Web.Utility.ConvertMinutesToHours(Convert.ToInt32(_Minutes)) == 0 || _Note.Length == 0)
                        image = String.Format(clearGif, new object[] { _AllocId, "false" });
                    else
                        image = String.Format(imgInputTemplate, new object[] { _AllocId, "false", "", imageUrl });
                    break;
            }
            if (textBox.IndexOf("style='display: none;") > 0)
            {
                image = String.Format(imgInputTemplate, new object[] { _AllocId, "false", "style='display: none;'", imageUrl });
            }
            Img.Text = image;
            Text.Text = textBox;
        }

        private string UnderOrOver()
        {
            if (_AvailableHours > RGA.Allocations.Web.Utility.ConvertMinutesToHours(_TotalMins))
                return "under";
            else if (_AvailableHours < RGA.Allocations.Web.Utility.ConvertMinutesToHours(_TotalMins))
                return "over";
            else
                return "";
        }

        public int WeekNumber
        {
            set { _WeekNumber = value; }
        }

        /// <summary>
        /// user id of logged in user
        /// </summary>
        public int UserId
        {
            set { _UserId = value; }
        }

        public int JobId
        {
            set { _JobId = value; }
        }

        /// <summary>
        /// the amount of minutes for this allocation
        /// </summary>
        public int Minutes
        {
            set { _Minutes = value; }
        }

        /// <summary>
        /// this is 0 when no record exists in the dbo.Allocations table
        /// </summary>
        public int AllocationId
        {
            set { _AllocId = value; }
        }

        /// <summary>
        /// this is the job start date
        /// </summary>
        public DateTime Start
        {
            set { _Start = value; }
        }

        /// <summary>
        /// this is the job end date
        /// </summary>
        public DateTime End
        {
            set { _End = value; }
        }

        public string Note
        {
            set { _Note = value; }
        }

        /// <summary>
        /// this is from the enum SecurityType in TeamAllocation.aspx
        /// </summary>
        public RgaWeb.SecurityType UserType
        {
            set { _UserType = value; }
        }

        public int Client
        {
            set { _ClientId = value; }
        }

        public int Available
        {
            set { _AvailableHours = value; }
        }

        public int Total
        {
            set { _TotalMins = value; }
        }
    }
}