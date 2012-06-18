using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Collections;
using System.Diagnostics;

namespace RGA.Allocations.Data
{
    public static class DeptGrid
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        public static DataSet DeptAllocationsGrid(int deptId, int startWeek, string resType, string title, int clientId, int region)
        {
            long start = DateTime.Now.Ticks;
            Debug.WriteLine("in DeptAllocationsGrid deptId: " + deptId + " startWeek: " + startWeek + " resType: " + resType +
                                " title: " + title + " clientId: " + clientId + " region: " + region);
            DataSet MasterDataSet = new DataSet();
            MasterDataSet.Clear();
            //try to remove certain rows for the various job/employee filters
            DataTable EmpsAndHours = AllocatedEmployees(deptId, startWeek, title, region);
            DataTable Jobs = ClientsAndJobs(clientId, startWeek);
            FilterData(EmpsAndHours, Jobs, resType);
            MasterDataSet.Tables.Add(EmpsAndHours);
            MasterDataSet.Tables.Add(Jobs);
            long end = DateTime.Now.Ticks;
            Debug.WriteLine("getting Dept Grid data took: " + (end - start).ToString("n") + " ticks\n\n");
            return MasterDataSet;
        }

        private static void FilterData(DataTable emps, DataTable jobs, string resType)
        {

             //notice that except for resType = ASSIGNED and jobType = All, these filters
             //all depends on both JobId and Userid data because all of these look at the 
             //allocations table, whose rows include jobid, userid fields.

            //restypes are: ASSIGNED, ALLOCATED, UNALLOCATED, OVERALLOCATED, UNDERALLOCATED
            long start = DateTime.Now.Ticks;
            ArrayList toRemove = new ArrayList();
            DataRow[] rows = null;

            //filter for "Allocated" jobs
            foreach (DataRow row in jobs.Rows)
            {
                int job = Convert.ToInt32(row["JobId"]);
                rows = emps.Select("JobId=" + job + " AND AnyMins>0");
                if (rows.Length == 0)
                    toRemove.Add(row);
            }
            if (toRemove.Count > 0)
                foreach (object row in toRemove)
                    jobs.Rows.Remove((DataRow)row);

            switch (resType)
            {
                case "ALLOCATED":
                    rows = emps.Select("HrsAllocated<=0 AND RealPerson='Y'");
                    foreach (DataRow row in rows)
                        emps.Rows.Remove(row);
                    break;
                case "UNALLOCATED":
                    rows = emps.Select("HrsAllocated>0 AND RealPerson='Y'");
                    foreach (DataRow row in rows)
                        emps.Rows.Remove(row);
                    break;
                case "OVERALLOCATED":
                    rows = emps.Select("HrsAllocated < (HrsAvailable*60) AND RealPerson='Y'");
                    foreach (DataRow row in rows)
                        emps.Rows.Remove(row);
                    break;
                case "UNDERALLOCATED":
                    //only show ppl who are allocd less hrs than their max
                    rows = emps.Select("HrsAllocated > (HrsAvailable*60) AND RealPerson='Y'");
                    foreach (DataRow row in rows)
                        emps.Rows.Remove(row);
                    break;
            }
            long end = DateTime.Now.Ticks;
            Debug.WriteLine("filterdata() in deptgrid took: " + (end - start).ToString("n") + " ticks");
        }

        private static DataTable AllocatedEmployees(int deptId, int startWeek, 
                                            string title, int region)
        {
            long start = DateTime.Now.Ticks;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetStoredProcCommand("ALOC_DeptGridEmpsList");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@deptId", DbType.Int32, deptId);
            db.AddInParameter(command, "@title", DbType.String, title);
            db.AddInParameter(command, "@weekNumber", DbType.Int32, startWeek);
            db.AddInParameter(command, "@regionId", DbType.Int32, region);

            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Emps";
            command.Dispose();
            long end = DateTime.Now.Ticks;
            Debug.WriteLine("getting emps list in deptgrid took: " + (end - start).ToString("n") + " ticks");
            return t;
        }

        private static DataTable ClientsAndJobs(int clientId, int weekNumber)
        {
            long start = DateTime.Now.Ticks;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetStoredProcCommand("ALOC_DeptGridJobsList");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@clientId", DbType.Int32, clientId);
            db.AddInParameter(command, "@weekNumber", DbType.Int32, weekNumber);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Jobs";
            command.Dispose();
            long end = DateTime.Now.Ticks;
            Debug.WriteLine("getting clients & jobs in deptgrid took: " + (end - start).ToString("n") + " ticks");
            return t;
        }

        public static DataTable GetEmpsForDept(int deptId, bool includeTbd)
        {
            if (deptId <= 0) deptId = -999;

            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = null;
            if (includeTbd)
                command = db.GetSqlStringCommand(@" SELECT  UserId, FirstName, LastName
                                                    FROM    AllocableUsers U
                                                    WHERE   DeptId=@dept_id
                                                    ORDER BY FirstName");
            else
                command = db.GetSqlStringCommand(@" SELECT  UserId, FirstName, LastName
                                                    FROM    AllocableUsers U
                                                    WHERE   DeptId=@dept_id AND realPerson='Y'
                                                    ORDER BY FirstName");
            command.CommandType = CommandType.Text;
            db.AddInParameter(command, "@dept_id", DbType.Int32, deptId);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Emps";
            command.Dispose();

            return t;
        }
    }
}
