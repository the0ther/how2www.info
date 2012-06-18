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
    public class EmpGrid
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        public static DataSet EmpAllocationsGrid(int empId, int startWeek)
        {
            DataSet retval = new DataSet();
            DataTable J = Jobs(empId, startWeek);
            DataTable C = Clients(empId, startWeek);
            DataTable A = Availability(empId, startWeek);
            retval.Tables.Add(J);
            retval.Tables.Add(C);
            retval.Tables.Add(A);
            return retval;
        }

        private static DataTable Jobs(int empId, int weekNumber)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetStoredProcCommand("ALOC_EmpGridJobsList");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@userId", DbType.Int32, empId);
            db.AddInParameter(command, "@weekNumber", DbType.Int32, weekNumber);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Jobs";
            command.Dispose();

            return t;
        }

        private static DataTable Clients(int empId, int weekNumber)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetStoredProcCommand("ALOC_EmpGridClientsList");
            db.AddInParameter(command, "@userId", DbType.Int32, empId);
            db.AddInParameter(command, "@weekNumber", DbType.Int32, weekNumber);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Clients";
            command.Dispose();

            return t;
        }

        private static DataTable Availability(int empId, int weekNumber)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetStoredProcCommand("ALOC_EmpGridAvailability");
            db.AddInParameter(command, "@userId", DbType.Int32, empId);
            db.AddInParameter(command, "@weekNumber", DbType.Int32, weekNumber);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Availability";
            command.Dispose();

            return t;            
        }

        public static DataSet TbdEmpAllocsGrid(int _EmpId, int _StartWeek)
        {
            DataSet retval = new DataSet();
            DataTable J = TbdJobs(_EmpId, _StartWeek);
            DataTable C = TbdClients(_EmpId, _StartWeek);
            DataTable A = Availability(_EmpId, _StartWeek);
            retval.Tables.Add(J);
            retval.Tables.Add(C);
            retval.Tables.Add(A);
            return retval;
        }

        private static DataTable TbdJobs(int empId, int _StartWeek)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetStoredProcCommand("ALOC_TbdJobs");
            db.AddInParameter(command, "@weekNumber", DbType.Int32, _StartWeek);
            db.AddInParameter(command, "@userId", DbType.Int32, empId);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Jobs";
            command.Dispose();

            return t;
        }

        private static DataTable TbdClients(int empId, int weekNumber)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetStoredProcCommand("ALOC_TbdClients");
            //db.AddInParameter(command, "@userId", DbType.Int32, empId);
            db.AddInParameter(command, "@weekNumber", DbType.Int32, weekNumber);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Clients";
            command.Dispose();

            return t;
        }
    }
}
