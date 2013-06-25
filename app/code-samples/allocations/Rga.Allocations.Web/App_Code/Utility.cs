using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using Jayrock.Json;
using System.IO;

namespace RGA.Allocations.Web
{
    /// <summary>
    /// EditAll = 0, EditUpcoming = 1, EditNone = 2
    /// </summary>
    public enum SecurityType { EditAll, EditUpcoming, EditNone };

    public static class Utility
    {
        public static Single ConvertMinutesToHours( int mins )
        {
            return Convert.ToSingle( mins ) / 60;
        }

        public static string IsNotBetween(object start, object end, object middle)
        {
            string retval = "";
            DateTime st, ed, mid;
            try
            {
                st = Convert.ToDateTime(start);
                ed = Convert.ToDateTime(end);
                mid = Convert.ToDateTime(middle);
                if (st != null && ed != null && mid!=null)
                    if (mid < st || mid > ed)
                        retval = "style='display: none;'";
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.StackTrace);
            }
            return retval;
        }

        public static JsonWriter CreateJsonWriter(TextWriter writer)
        {
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.PrettyPrint = true;
            return jsonWriter;
        }
    }
}
