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
    public class Employee
    {

        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        public static IDataReader Employees(int deptId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand("SELECT FirstName + ' ' + LastName AS FullName, UserId FROM AllocableUsers WHERE DeptId=" + deptId + " AND Active=1 ORDER BY FullName");
            command.CommandType = CommandType.Text;

            return db.ExecuteReader(command);
        }
    }
}
