using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
//using Jayrock.Json;

namespace RGA.Allocations.Data
{
    public static class TeamGrid
    {
        private static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Allocations_Dev"].ConnectionString;
 
        public static DataSet TeamAllocationsGrid( int clientId, int startWeek, int departmentId, int clientGroupId, string resourceType, int region )
        {
            // Set up master DataSet
            DataSet MasterDataSet = new DataSet();

            // Get all users assigned/allocated to client's jobs
            MasterDataSet.Tables.Add( AllocatedUsers( clientId, startWeek, departmentId, clientGroupId, resourceType, region ) );

            // Get all jobs with assigned/allocated users 
            MasterDataSet.Tables.Add( AllocatedJobs( clientId, startWeek, resourceType ) );

            return MasterDataSet;

        }

        private  static DataTable AllocatedUsers( int clientId, int startWeek, int departmentId, 
                                    int clientGroupId, string resourceType, int region )
        {
            SqlDatabase db = new SqlDatabase( connString );

            DbCommand command = db.GetStoredProcCommand( "ALOC__TeamGridUserListSelect" );
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter( command, "@ClientId", DbType.Int32, clientId );
            db.AddInParameter( command, "@StartWeek", DbType.Int32, startWeek );
            db.AddInParameter( command, "@DepartmentId", DbType.Int32, departmentId );
            db.AddInParameter( command, "@ClientGroupId", DbType.Int32, clientGroupId );
            db.AddInParameter( command, "@ResourceType", DbType.String, resourceType );
            db.AddInParameter(command, "@RegionId", DbType.Int32, region);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet( command ).Tables[0].Copy();
            t.TableName = "User";
            command.Dispose();

            return t;

        }

        private static DataTable AllocatedJobs( int clientId, int startWeek, string resourceType )
        {
            SqlDatabase db = new SqlDatabase( connString );

            DbCommand command = db.GetStoredProcCommand( "ALOC__TeamGridJobListSelect" );
            command.CommandType = CommandType.StoredProcedure;
            db.AddInParameter( command, "@ClientId", DbType.Int32, clientId );
            db.AddInParameter( command, "@StartWeek", DbType.Int32, startWeek );
            db.AddInParameter( command, "@Type", DbType.String, resourceType );

            DataTable t = new DataTable();
            t = db.ExecuteDataSet( command ).Tables[0].Copy();
            t.TableName = "Job";
            command.Dispose();

            return t;

        }

        public static DataTable ClientTeams(int clientId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand(@"
                    SELECT  TeamID, Name, Description 
                    FROM    ALOC_Teams 
                    WHERE   ClientID=@client_id 
                    ORDER BY Name
                ");
            command.CommandType = CommandType.Text;
            db.AddInParameter(command, "@client_id", DbType.Int32, clientId);
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Teams";
            command.Dispose();
            return t;
        }

        public static DataTable ClientTeamMembers(int teamId)
        {
            SqlDatabase db = new SqlDatabase(connString);

            DbCommand command = db.GetSqlStringCommand(@"
                    SELECT  M.TeamMemberId, U.UserId, U.FirstName + ' ' + U.LastName AS FullName, D.ShortName 
                    FROM	ALOC_Teams T LEFT JOIN ALOC_TeamMembers M ON T.TeamId=M.TeamId 
                            INNER JOIN AllocableUsers U ON M.EmployeeId=U.UserId 
                            INNER JOIN AllDepartments D ON U.DeptId=D.DeptId 
                    WHERE T.TeamId=@team_id ORDER BY FullName");
            command.CommandType = CommandType.Text;
            db.AddInParameter(command, "@team_id", DbType.Int32, teamId);

            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Team";
            command.Dispose();

            return t;
        }

        public static int RemoveFromClientTeam(int teamMemberId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand(@"
                                DELETE FROM ALOC_TeamMembers 
                                WHERE TeamMemberId=@TeamMemberId");
            command.CommandType = CommandType.Text;
            db.AddInParameter(command, "@TeamMemberId", DbType.Int32, teamMemberId);
            int retval = db.ExecuteNonQuery(command);
            command.Dispose();
            return retval;
        }

        public static int AddToClientTeam(int teamId, int userId)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand(@"
                            INSERT INTO ALOC_TeamMembers (TeamId, EmployeeId) 
                            VALUES (@team_id, @emp_id);");
            command.CommandType = CommandType.Text;
            db.AddInParameter(command, "@team_id", DbType.Int32, teamId);
            db.AddInParameter(command, "@emp_id", DbType.Int32, userId);
            int retval = db.ExecuteNonQuery(command);
            command.Dispose();
            return retval;
        }

        public static DataTable GetDepts()
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand command = db.GetSqlStringCommand(@"
                    SELECT DeptId, Name 
                    FROM AllDepartments 
                    WHERE Active=1 AND normBillsTime=1
                    ORDER BY Name");
            command.CommandType = CommandType.Text;
            DataTable t = new DataTable();
            t = db.ExecuteDataSet(command).Tables[0].Copy();
            t.TableName = "Depts";
            command.Dispose();

            return t;
        }

        public static int CreateNewClientTeam(string name, int clientid, string desc)
        {
            int retval = -1;
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand(@"
                INSERT INTO ALOC_Teams  (Name, ClientId, Description) 
                VALUES                  (@name, @client, @desc)
            ");
            cmd.CommandType = CommandType.Text;
            db.AddInParameter(cmd, "@name", DbType.String, name);
            db.AddInParameter(cmd, "@client", DbType.Int32, clientid);
            db.AddInParameter(cmd, "@desc", DbType.String, desc);
            db.ExecuteNonQuery(cmd);
            cmd.Dispose();
            cmd = db.GetSqlStringCommand("SELECT MAX(TeamId) FROM ALOC_TEAMS");
            object obj = db.ExecuteScalar(cmd);
            if (obj != DBNull.Value)
                retval = Convert.ToInt32(obj);
            return retval;
        }

        public static void EditClientTeam(int teamid, string name, string desc)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand(@"
                    UPDATE  ALOC_Teams 
                    SET     Name=@name, Description=@desc 
                    WHERE   TeamId=@team_id
            ");
            cmd.CommandType = CommandType.Text;
            db.AddInParameter(cmd, "@name", DbType.String, name);
            db.AddInParameter(cmd, "@desc", DbType.String, desc);
            db.AddInParameter(cmd, "@team_id", DbType.Int32, teamid);
            db.ExecuteNonQuery(cmd);
            cmd.Dispose();
        }

        public static void DeleteClientTeam(int teamid)
        {
            SqlDatabase db = new SqlDatabase(connString);
            DbCommand cmd = db.GetSqlStringCommand(@"DELETE FROM ALOC_TeamMembers WHERE TeamId=@team_id");
            cmd.CommandType = CommandType.Text;
            db.AddInParameter(cmd, "@team_id", DbType.Int32, teamid);
            db.ExecuteNonQuery(cmd);
            cmd = db.GetSqlStringCommand("DELETE FROM ALOC_Teams WHERE TeamId=@team_id");
            cmd.CommandType = CommandType.Text;
            db.AddInParameter(cmd, "@team_id", DbType.Int32, teamid);
            db.ExecuteNonQuery(cmd);
            cmd.Dispose();
        }
    }
}
