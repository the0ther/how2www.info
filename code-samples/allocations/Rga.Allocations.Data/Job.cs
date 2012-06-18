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
    public class Job
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        private int _JobId;

        public int JobId
        {
            get { return _JobId; }
            set { _JobId = value; }
        }
        private string _JobCode;

        public string JobCode
        {
            get { return _JobCode; }
            set { _JobCode = value; }
        }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private int _ClientId;

        public int ClientId
        {
            get { return _ClientId; }
            set { _ClientId = value; }
        }
        private int _Active;

        public int Active
        {
            get { return _Active; }
            set { _Active = value; }
        }
   
        public static IDataReader Jobs( int clientId, int userId)
        {
            SqlDatabase db = new SqlDatabase( connString );

            DbCommand command = db.GetStoredProcCommand( "ALOC__JobListSelect" );
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter( command, "@ClientId", DbType.Int32, clientId );
            db.AddInParameter( command, "@UserId", DbType.Int32, userId );
            return  db.ExecuteReader( command );
        }

        public static IDataReader JobsAssignedTo(int clientId, int userId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetStoredProcCommand("ALOC_JobsAssignedTo");
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter(command, "@ClientId", DbType.Int32, clientId);
            db.AddInParameter(command, "@UserId", DbType.Int32, userId);
            return db.ExecuteReader(command);
        }

        public static IDataReader AllJobs(int clientId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand(@"
                SELECT  JobId, Name FROM AllOpenJobs J  
                WHERE   Active=1 AND ClientId=@clientId
                ORDER BY Name");
            db.AddInParameter(command, "@clientId", DbType.Int32, clientId);
            return db.ExecuteReader(command);
        }
    }
}
