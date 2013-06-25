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
    public class ClientGroup
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        private int _ClientGroupId;

        public int ClientGroupId
        {
            get { return _ClientGroupId; }
            set { _ClientGroupId = value; }
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

        public static IDataReader ClientGroups( int clientId )
        {
            SqlDatabase db = new SqlDatabase( connString );
            DbCommand command = db.GetStoredProcCommand( "ALOC__ClientGroupSelectList" );
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter( command, "@ClientId", DbType.Int32, clientId );

            return db.ExecuteReader( command );
        }
    }
}
