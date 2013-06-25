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

namespace Allocations
{
    public partial class AllocationNote : Allocations.simpPage
    {
        private int _AllocId = -1;
        private bool _Enabled = true;
        private delegate bool IsInGroup(ref DBHelper db, string str);

        protected void Page_Load(object sender, EventArgs e)
        {
            int allocId = 0;
            IsInGroup groupTest = null;

            //it's only .AllocId and .Enabled which need to be set here
            if (Request.QueryString["AllocId"] != null)
                Int32.TryParse(Request.QueryString["AllocId"],out allocId);
            if (Request.QueryString["Enabled"] != null)
                Boolean.TryParse(Request.QueryString["Enabled"], out _Enabled);

            if (Request.Url.Host.ToLower() == "localhost")
            {
                //base.login_user_id = 40123; //user
                //base.login_user_id = 40110; //exec producer -- skip
                //base.login_user_id = 272; //producer -- elyse
                //base.login_user_id = 205; //coordinator
                //base.login_user_id = 144; //administrator -- randy
                groupTest = TestGroupMembership;
            }
            else
            {
                base.Page_Load("/newallocations/AllocationNote.aspx");
                groupTest = base.testmembership;
            }

            /*
            DBHelper db = new DBHelper();
            if (groupTest.Invoke(ref db, "A3Coordinator"))
                this.Enabled = true;
            else if (groupTest.Invoke(ref db, "A2Administrator"))
                this.Enabled = true;
            else if (groupTest.Invoke(ref db, "A8Producer"))
                this.Enabled = true;
            else if (groupTest.Invoke(ref db, "A5ExecProducer"))
                this.Enabled = true;
            else
                this.Enabled = false;
            */
            this.Enabled = _Enabled;
            this.AllocId = allocId;
        }

        private int AllocId
        {
            set
            {
                _AllocId = value;
                ViewState["AllocNoteId"] = _AllocId;
                if (!Page.IsPostBack)
                    BindControls();
            }
            get
            {
                if (ViewState["AllocNoteId"] != null)
                {
                    _AllocId = Convert.ToInt32(ViewState["AllocNoteId"]);
                }
                return _AllocId;
            }
        }

        private bool Enabled
        {
            set
            {
                _Enabled = value;
                if (_Enabled == true)
                {
                    radBtns.Enabled = true;
                    Note.Enabled = true;
                    CharLimit.Enabled = true;
                    Ok.Enabled = true;
                }
                else
                {
                    radBtns.Enabled = false;
                    Note.Enabled = false;
                    CharLimit.Enabled = false;
                    Ok.Enabled = false;
                }
            }
        }

        protected void Ok_OnClick(object sender, EventArgs e)
        {
            if (Note.Text.Length > 120)
            {
                errorMsg.Text = "alert('Notes cannot be longer than 120 characters.');";
            }
            else
            {
                errorMsg.Text = "";
                using (SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    cnxn.Open();
                    SqlCommand cmd = new SqlCommand(@"  UPDATE Allocations SET
                                                    RequestingSunday=@sun, RequestingMonday=@mon, RequestingTuesday=@tue,
                                                    RequestingWednesday=@wed, RequestingThursday=@thur,
                                                    RequestingFriday=@fri, RequestingSaturday=@sat,
                                                    AllocNote=@note
                                                    WHERE   AllocationId=@alloc_id", cnxn);
                    cmd.Parameters.AddWithValue("@sun", radBtns.Items[6].Selected);
                    cmd.Parameters.AddWithValue("@mon", radBtns.Items[0].Selected);
                    cmd.Parameters.AddWithValue("@tue", radBtns.Items[1].Selected);
                    cmd.Parameters.AddWithValue("@wed", radBtns.Items[2].Selected);
                    cmd.Parameters.AddWithValue("@thur", radBtns.Items[3].Selected);
                    cmd.Parameters.AddWithValue("@fri", radBtns.Items[4].Selected);
                    cmd.Parameters.AddWithValue("@sat", radBtns.Items[5].Selected);
                    cmd.Parameters.AddWithValue("@note", Note.Text);
                    cmd.Parameters.AddWithValue("@alloc_id", AllocId);
                    cmd.ExecuteNonQuery();
                    if (cnxn.State == ConnectionState.Open)
                        cnxn.Close();
                }
                string showOnBtn = Note.Text == string.Empty ? "false" : "true";
                foreach (ListItem btn in radBtns.Items)
                    if (btn.Selected)
                        showOnBtn = "true";
                string closeFunc = @"
                        var popupiframe = window.parent.document.getElementById('notesIFrame');
                        var allocId = popupiframe.src.substring(popupiframe.src.indexOf('AllocId=')+8,popupiframe.src.lastIndexOf('&'));
                        var enabled = popupiframe.src.substring(popupiframe.src.indexOf('Enabled=')+8);
                    
                        popupiframe.src = '';
                    
                        var popuppanel = window.parent.document.getElementById('notesPanel');
                        popuppanel.style.display = 'none'; 
                	
	                    //change the image from gray to green if there is a note
                        if (" + showOnBtn + @")
                        {
                	        var imgBtn = window.parent.document.getElementById('allocNote_' + allocId + '_' + enabled);
                            //var imgBtn = window.parent.document.getElementById('allocNote_' + allocId + '_false');
                	        imgBtn.src = 'Img/icon_note_on.gif';
                        }
                        else
                        {
                	        var imgBtn = window.parent.document.getElementById('allocNote_' + allocId + '_false');
                	        imgBtn.src = 'Img/icon_note_off.gif';
                        }
                	";
                notesCloser.Text = closeFunc;
            }
        }

        private void BindControls()
        {
            using (SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                cnxn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT    RequestingSunday, RequestingMonday, RequestingTuesday, RequestingWednesday, 
                                                            RequestingThursday, RequestingFriday, RequestingSaturday, 
                                                            AllocNote 
                                                  FROM      Allocations
                                                  WHERE     AllocationId=@alloc_id", cnxn);
                cmd.Parameters.AddWithValue("@alloc_id", AllocId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //these used to be 0,1,2,3,4,5,6 but now that Sunday was moved to the end these are 1,2,3,4,5,6,0
                    radBtns.Items[0].Selected = reader[0] == DBNull.Value ? false : Convert.ToBoolean(reader[1]);
                    radBtns.Items[1].Selected = reader[1] == DBNull.Value ? false : Convert.ToBoolean(reader[2]);
                    radBtns.Items[2].Selected = reader[2] == DBNull.Value ? false : Convert.ToBoolean(reader[3]);
                    radBtns.Items[3].Selected = reader[3] == DBNull.Value ? false : Convert.ToBoolean(reader[4]);
                    radBtns.Items[4].Selected = reader[4] == DBNull.Value ? false : Convert.ToBoolean(reader[5]);
                    radBtns.Items[5].Selected = reader[5] == DBNull.Value ? false : Convert.ToBoolean(reader[6]);
                    radBtns.Items[6].Selected = reader[6] == DBNull.Value ? false : Convert.ToBoolean(reader[0]);
                    Note.Text = reader[7] == DBNull.Value ? "" : reader[7].ToString();
                }
                else
                {
                    radBtns.Items[0].Selected = false;
                    radBtns.Items[1].Selected = false;
                    radBtns.Items[2].Selected = false;
                    radBtns.Items[3].Selected = false;
                    radBtns.Items[4].Selected = false;
                    radBtns.Items[5].Selected = false;
                    radBtns.Items[6].Selected = false;
                    Note.Text = string.Empty;
                }
                if (cnxn.State == ConnectionState.Open)
                    cnxn.Close();
            }
        }

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