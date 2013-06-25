using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace RGA.Allocations.Data
{
    public static class Utility
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        public static int CurrectWeek()
        {
            SqlDatabase db = new SqlDatabase( connString );
            DbCommand command = db.GetStoredProcCommand( "getWeekNr" );
            command.CommandType = CommandType.StoredProcedure;
            //the +1 below is to correct for an apparent off-by-one error in the stored procedure
            return Convert.ToInt32( db.ExecuteScalar( command ) ) + 1;
        }

        public static DateTime CurrectWeekStartDate( int currentWeek )
        {
            SqlDatabase db = new SqlDatabase( connString );
            DbCommand command = db.GetStoredProcCommand( "smWkNmStr" );
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter( command, "@week_no", DbType.Int32, currentWeek );

            return Convert.ToDateTime( db.ExecuteScalar( command ) );
        }

        public static DateTime GetJobStartDate(int jobId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand("SELECT projStartDate FROM AllOpenJobs WHERE JobId=" + jobId);
            DateTime retval = DateTime.MaxValue;
            object obj = db.ExecuteScalar(command);
            if (obj != DBNull.Value)
                retval = Convert.ToDateTime(obj);
            return retval;
        }

        public static DateTime GetJobEndDate(int jobId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand("SELECT currentEndDate FROM AllOpenJobs WHERE JobId=" + jobId);
            DateTime retval = DateTime.MinValue;
            object obj = db.ExecuteScalar(command);
            if (obj != DBNull.Value)
                retval = Convert.ToDateTime(obj);
            return retval;
        }

        public static int GetDeptIdForUser(int userId)
        {
            int retval = -1;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand("SELECT DeptId FROM AllocableUsers WHERE UserId=" + userId);
            command.CommandType = CommandType.Text;
            object obj = db.ExecuteScalar(command);
            if (obj != DBNull.Value)
                retval = Convert.ToInt32(obj);
            return retval;
        }

        public static int GetClientIdForJob(int jobId)
        {
            int retval = -1;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand("SELECT ClientId FROM AllOpenJobs WHERE JobId=" + jobId);
            command.CommandType = CommandType.Text;
            object obj = db.ExecuteScalar(command);
            if (obj != DBNull.Value)
                retval = Convert.ToInt32(obj);
            return retval;
        }

        public static string GetTitleForEmp(int userId)
        {
            string retval = string.Empty;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand(@"
                    SELECT  T.TitleName
                    FROM    JobTitles T INNER JOIN AllocableUsers U ON U.currentTitleId=T.TitleId
                    WHERE   U.UserId=" + userId);
            command.CommandType = CommandType.Text;
            object obj = db.ExecuteScalar(command);
            if (obj != null && obj != DBNull.Value)
                retval = obj.ToString();
            return retval;
        }
    }
}
