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
    public class User
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;

        private int _UserId;

        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        private int _DeptId;

        public int DeptId
        {
            get { return _DeptId; }
            set { _DeptId = value; }
        }
        private string _FirstName;

        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }
        private string _LastName;

        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }
        private bool _Active;

        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
        }
    }
}
