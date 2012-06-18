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
    public static class RegionGrid
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        public static DataSet RegionAllocationsGrid(int region, int startWeek, int deptId, string resType)
        {
            DataSet MasterDataSet = new DataSet();
            MasterDataSet.Tables.Add(AllocatedUsers(region, startWeek, deptId, resType));
            MasterDataSet.Tables.Add(AllocatedJobs(region, startWeek));
            return MasterDataSet;
        }

        private static DataTable AllocatedUsers(int region, int startWeek, int deptId, string resType)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetStoredProcCommand("ALOC_RegionGridUserListSelect");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@RegionId", DbType.Int32, region);
            db.AddInParameter(command, "@StartWeek", DbType.Int32, startWeek);
            db.AddInParameter(command, "@DepartmentId", DbType.Int32, deptId);
            db.AddInParameter(command, "@ResourceType", DbType.String, resType);
            DataTable dt = null;
            dt = db.ExecuteDataSet(command).Tables[0].Copy();
            dt.TableName = "User";
            command.Dispose();
            return dt;
        }

        private static DataTable AllocatedJobs(int region, int startWeek)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetStoredProcCommand("ALOC_RegionGridJobListSelect");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@RegionId", DbType.Int32, region);
            db.AddInParameter(command, "@StartWeek", DbType.Int32, startWeek);
            DataTable dt = null;
            dt = db.ExecuteDataSet(command).Tables[0].Copy();
            dt.TableName = "Jobs";
            command.Dispose();
            return dt;
        }
    }
}
