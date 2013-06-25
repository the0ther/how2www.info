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
    public static class ProjectGrid
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        public static DataSet ProjectAllocationsGrid(int clientId, int startWeek, int jobId, string resType, int region)
        {
            DataSet MasterDataSet = new DataSet();

            // Get all users assigned/allocated to client's jobs
            MasterDataSet.Tables.Add(AllocatedDepts(clientId, startWeek, jobId));

            // Get all jobs with assigned/allocated users 
            MasterDataSet.Tables.Add(AllocatedEmps(clientId, startWeek, jobId, resType, region));

            return MasterDataSet;
        }

        private static DataTable AllocatedDepts(int clientId, int startWeek, int jobId)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetStoredProcCommand("ALOC_ProjectGridDeptSelect");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@client_id", DbType.Int32, clientId);
            db.AddInParameter(command, "@start_week", DbType.Int32, startWeek);
            db.AddInParameter(command, "@job_id", DbType.Int32, jobId);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Depts";
            command.Dispose();

            return t;
        }

        private static DataTable AllocatedEmps(int clientId, int startWeek, int jobId, string resType, int region)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetStoredProcCommand("ALOC_ProjectGridEmpsSelect");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@client_id", DbType.Int32, clientId);
            db.AddInParameter(command, "@start_week", DbType.Int32, startWeek);
            //TODO: why is this setting @dept_id to Zero?
            db.AddInParameter(command, "@dept_id", DbType.Int32, 0);
            db.AddInParameter(command, "@job_id", DbType.Int32, jobId);
            db.AddInParameter(command, "@resource_type", DbType.String, resType);
            db.AddInParameter(command, "@region", DbType.Int32, region);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Emps";
            command.Dispose();

            return t;
        }
    }
}
