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
    public class Allocation
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        private int _AllocationId;

        public int AllocationId
        {
            get { return _AllocationId; }
            set { _AllocationId = value; }
        }
        private int _UserId;

        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        private int _JobId;

        public int JobId
        {
            get { return _JobId; }
            set { _JobId = value; }
        }
        private int _WeekNum;

        public int WeekNum
        {
            get { return _WeekNum; }
            set { _WeekNum = value; }
        }
        private int _Mins;

        public int Mins
        {
            get { return _Mins; }
            set { _Mins = value; }
        }
        private int _Status;

        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public static int[] Update( int employeeId, int jobId, int weekNum, int allocationId, int mins, int doNotAssign)
        {
            int[] retval = new int[] { -1, -1 };
            SqlDatabase db = new SqlDatabase( connString );
            DbCommand command = db.GetStoredProcCommand( "ALOC__AllocationUpdate" );
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter( command, "@EmployeeId", DbType.Int32, employeeId );
            db.AddInParameter( command, "@JobId", DbType.Int32, jobId );
            db.AddInParameter( command, "@WeekNum", DbType.Int32, weekNum );
            db.AddInParameter(command, "@DoNotAssign", DbType.Int32, doNotAssign);
            //db.AddInParameter( command, "@AllocationId", DbType.Int32, allocationId );
            db.AddInParameter( command, "@Mins", DbType.Int32, mins );
            //object val = db.ExecuteScalar(command);
            IDataReader rr = db.ExecuteReader(command);
            //if (rr.NextResult())
            if (rr.Read())
            {
                retval[0] = Convert.ToInt32(rr["AllocId"].ToString());
                retval[1] = Convert.ToInt32(rr["NoteLength"].ToString());
            }
            command.Dispose();
            return retval;
        }

        public static bool IsLocked(int week_number, int client_id)
        {
            bool retval = false;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand("SELECT '' FROM ALOC_Locks WHERE WeekNumber=@week_num AND ClientId=@client_id");
            cmd.CommandType = CommandType.Text;
            db.AddInParameter(cmd, "@week_num", DbType.Int32, week_number);
            db.AddInParameter(cmd, "@client_id", DbType.Int32, client_id);

            IDataReader rdr = db.ExecuteReader(cmd);
            if (rdr.Read())
                retval = true;
            
            if (!rdr.IsClosed)
                rdr.Close();
            cmd.Dispose();
            return retval;
        }

        public static void LockAllocation(int week_number, int client_id)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand("SELECT '' FROM ALOC_Locks WHERE WeekNumber=@week_num AND ClientId=@client_id");
            cmd.CommandType = CommandType.Text;
            db.AddInParameter(cmd, "@week_num", DbType.Int32, week_number);
            db.AddInParameter(cmd, "@client_id", DbType.Int32, client_id);
            DbDataReader rdr = cmd.ExecuteReader();
            if (!rdr.Read())
            {
                DbCommand cmd2 = db.GetSqlStringCommand("INSERT INTO ALOC_Locks (WeekNumber, ClientId) VALUES (@week_num, @client_id)");
                cmd2.CommandType = CommandType.Text;
                db.AddInParameter(cmd2, "@week_num", DbType.Int32, week_number);
                db.AddInParameter(cmd2, "@client_id", DbType.Int32, client_id);
                db.ExecuteNonQuery(cmd2);
                cmd2.Dispose();
            }
            cmd.Dispose();
        }

        public static void UnlockAllocation(int week_number, int client_id)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand("DELETE FROM ALOC_Locks WHERE WeekNumber=@week_num AND ClientId=@client_id");
            cmd.CommandType = CommandType.Text;
            db.AddInParameter(cmd, "@week_num", DbType.Int32, week_number);
            db.AddInParameter(cmd, "@client_id", DbType.Int32, client_id);
            db.ExecuteNonQuery(cmd);
            cmd.Dispose();
        }
    }
}
