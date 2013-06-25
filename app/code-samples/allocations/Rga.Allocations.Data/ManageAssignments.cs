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
    public class ManageAssignments
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        public static DataTable GetResources(int jobId, int deptId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            string sql = @" SELECT	U.UserId, U.FirstName + ' ' + U.LastName AS FullName, 
		                            t.TitleName as Title
                            FROM	AllocableUsers U LEFT JOIN JobTitles AS t ON U.currentTitleID=t.TitleID
                            WHERE	U.Active=1 AND U.UserId NOT IN (
                                    SELECT UserId FROM Assignments WHERE JobId=@job_id
                                    AND (EndDate IS NULL OR EndDate>DATEADD(s, 1, CURRENT_TIMESTAMP))) 
                                    AND U.DeptId=@dept_id AND realPerson='Y'
		                            AND UserId NOT IN (
			                            SELECT	UserId
			                            FROM	timeEntry
			                            WHERE	JobId=@job_id AND UserId=U.UserId AND (TimeSpan IS NULL OR TimeSpan > 0)
		                            )
                            ORDER BY FullName";

            DbCommand command = db.GetSqlStringCommand(sql);
            db.AddInParameter(command, "@job_id", DbType.Int32, jobId);
            db.AddInParameter(command, "@dept_id", DbType.Int32, deptId);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Resources";
            command.Dispose();
            return t;
        }

        public static DataTable GetAssigned(int jobId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            string sql = @" SELECT  U.UserId, U.FirstName + ' ' + U.LastName AS FullName,
                                    D.Name AS DeptName, TimeBilled = 
			                            CASE
				                            WHEN	A.EndDate IS NULL OR A.EndDate>DATEADD(s, 2, CURRENT_TIMESTAMP) THEN 0
				                            ELSE	1
			                            END
                            FROM    AllocableUsers U INNER JOIN AllDepartments D ON U.DeptId=D.DeptId
		                            INNER JOIN Assignments A ON (U.UserId=A.UserId AND A.JobId=@job_id)
                            WHERE   realPerson='Y'   
                            ORDER BY Fullname
                        ";
            DbCommand cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(cmd).Tables[0].Copy();
            t.TableName = "Assigned";
            cmd.Dispose();
            return t;
        }

        public static void Assign(int jobId, int userId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetStoredProcCommand("ALOC_Assign");
            db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
            db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
            db.ExecuteNonQuery(cmd);
            cmd.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="userId"></param>
        /// <param name="existing"></param>
        /// <returns>0 if user being unassigned has never billed time to the job, 
        /// 1 if user has billed time to the job</returns>
        public static int Unassign(int jobId, int userId, string existing)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = null;
            if (existing == "delete")
            {
                cmd = db.GetSqlStringCommand(@" DELETE FROM Allocations WHERE UserId=@user_id AND JobId=@job_id");
                db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
                db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
                db.ExecuteNonQuery(cmd);
            }
            else if (existing == "move")
            {
                MoveAllocations(userId, jobId);
            }
            int retval = 0;
            
            cmd = db.GetStoredProcCommand("ALOC_Unassign");
            db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
            db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
            object result = db.ExecuteScalar(cmd);
            if (result != null && result != DBNull.Value)
                retval = Convert.ToInt32(result);
            cmd.Dispose();
            return retval;
        }

        private static void MoveAllocations(int userId, int jobId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand(@"SELECT UserId 
                                                    FROM    AllocableUsers 
                                                    WHERE   DeptId=(SELECT DeptId FROM AllocableUsers WHERE UserId=@user_id)
                                                            AND FirstName='TBD' AND realPerson='N'");
            db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
            object val = db.ExecuteScalar(cmd);
            int tbdUserId = -1;
            if (val != null && val != DBNull.Value)
                tbdUserId = Convert.ToInt32(val);
            if (tbdUserId < 0)
                throw new Exception("Could not find TBD user in same department as user with userid " + userId);

            cmd = db.GetSqlStringCommand(@" SELECT  AllocationId, WeekNumber
                                            FROM    Allocations
                                            WHERE   JobId=@job_id AND UserId=@tbd_user AND WeekNumber>=dbo.fnGetWeekNumber()+1");
            db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
            db.AddInParameter(cmd, "@tbd_user", DbType.Int32, tbdUserId);
            DataTable tbdAllocs = db.ExecuteDataSet(cmd).Tables[0];
            //
            foreach (DataRow row in tbdAllocs.Rows)
            {
                //add the existing user's alloc'd minutes to these
                //TODO: why would this ever try to put NULL into AnyMins???
                cmd = db.GetSqlStringCommand(@" UPDATE  Allocations
                                                SET     AnyMins=IsNull(AnyMins,0)+IsNull((SELECT IsNull(AnyMins,0) FROM Allocations 
                                                                WHERE UserId=@user_id AND JobId=@job_id AND WeekNumber=@week_num),0)
                                                WHERE   AllocationId=@alloc_id");
                db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
                db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
                db.AddInParameter(cmd, "@week_num", DbType.Int32, row["WeekNumber"]);
                db.AddInParameter(cmd, "@alloc_id", DbType.Int32, row["AllocationId"]);
                db.ExecuteNonQuery(cmd);
                //now delete the allocation for the user
                cmd = db.GetSqlStringCommand(@" DELETE FROM Allocations WHERE UserId=@user_id AND JobId=@job_id AND WeekNumber=@week_num");
                db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
                db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
                db.AddInParameter(cmd, "@week_num", DbType.Int32, row["WeekNumber"]);
                db.ExecuteNonQuery(cmd);
            }
            //we've now moved any colliding allocation records...do an update of the user-id on the rest
            cmd = db.GetSqlStringCommand(@" UPDATE  Allocations 
                                            SET     UserId=@tbd_id, AllocNote='' 
                                            WHERE   JobId=@job_id AND UserId=@user_id AND WeekNumber>=dbo.fnGetWeekNumber()+1");
            db.AddInParameter(cmd, "@tbd_id", DbType.Int32, tbdUserId);
            db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
            db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
            db.ExecuteNonQuery(cmd);
        }

        public static bool HasAllocations(int jobId, int userId)
        {
            bool retval = false;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand(@"   SELECT  COUNT(AllocationId) 
                                                        FROM    Allocations
                                                        WHERE   UserId=@user_id AND JobId=@job_id AND WeekNumber>=dbo.fnGetWeekNumber()+1");
            db.AddInParameter(cmd, "@job_id", DbType.Int32, jobId);
            db.AddInParameter(cmd, "@user_id", DbType.Int32, userId);
            object result = db.ExecuteScalar(cmd);
            if (result != null && result != DBNull.Value)
            {
                if(Convert.ToInt32(result)>0)
                    retval = true;
            }
            cmd.Dispose();
            return retval;
        }
    }
}
