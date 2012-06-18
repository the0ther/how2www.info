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
    public class Department
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        private int _DepartmentId;

        public int DepartmentId
        {
            get { return _DepartmentId; }
            set { _DepartmentId = value; }
        }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private bool _Active;

        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
        }

        public static IDataReader Deparments(int clientId)
        {
            SqlDatabase db = new SqlDatabase( connString );
            DbCommand command = db.GetStoredProcCommand( "ALOC__DepartmentSelectList" );
            db.AddInParameter( command, "@ClientId", DbType.Int32, clientId );
            command.CommandType = CommandType.StoredProcedure;

            return db.ExecuteReader( command );
        }

        public static IDataReader Departments()
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand("SELECT DeptId, Name FROM AllDepartments D WHERE D.Active=1 AND D.NormBillsTime=1 ORDER BY Name");
            command.CommandType = CommandType.Text;
            return db.ExecuteReader(command);
        }
    }
}
