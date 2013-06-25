using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.ComponentModel;

namespace RGA.Allocations.Data
{
    [DataObjectAttribute()]
    public class Client
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        private int _ClientId;

        public int ClientId
        {
            get { return _ClientId; }
            set { _ClientId = value; }
        }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [DataObjectMethod( DataObjectMethodType.Select, true )]
        public static IDataReader  Clients( string type, int userId )
        {
            SqlDatabase db = new SqlDatabase( connString );
            DbCommand command = db.GetStoredProcCommand( "ALOC__ClientSelectList" );
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter( command, "@Type", DbType.String, type );
            db.AddInParameter( command, "@UserId", DbType.Int32, userId );

            return db.ExecuteReader( command );
        }


        public Client( int clientId, string name )
        {
            _ClientId = clientId;
            _Name = name;
        }
    }
}
