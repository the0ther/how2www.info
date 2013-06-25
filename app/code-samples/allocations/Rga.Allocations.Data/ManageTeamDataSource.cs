using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace RGA.Allocations.Data
{
    /// <summary>
    /// Summary description for ManageTeamDataSource
    /// </summary>
    public static class ManageTeamDataSource
    {
        public static ICollection<ManageTeamResourceRow> GetEmployees(int dept_id, int client_id, int team_id)
        {
            Debug.WriteLine("team id is: " + team_id);
            Debug.WriteLine("dept id is: " + dept_id);
            Debug.WriteLine("client id is: " + client_id);
            //HACK: this is here because when you try to fill the grid with people before the dept dropdown is databound, you get 0 results returned
            if (dept_id == 0) dept_id = 8;
            List<ManageTeamResourceRow> retval = new List<ManageTeamResourceRow>();
            SqlConnection cnxn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
            cnxn.Open();
            SqlCommand cmd = null;
            if (client_id > 0)
            {
                cmd = new SqlCommand(@"SELECT U.UserId, U.FirstName + ' ' + U.LastName AS FullName
                                    FROM AllocableUsers U
                                    WHERE U.DeptId=@dept_id AND U.Active=1 AND U.UserId IN (
                                        SELECT UserId 
                                        FROM Assignments A INNER JOIN AllOpenJobs J ON A.JobId=J.JobId
                                                INNER JOIN AllActiveClients C ON J.ClientId=C.ClientId
                                        WHERE C.ClientId=@client_id AND J.Active=1
                                    ) AND U.UserId NOT IN (SELECT EmployeeId FROM ALOC_TeamMembers WHERE TeamId=@team_id)
                                    ORDER BY FullName", cnxn);
                cmd.Parameters.AddWithValue("@client_id", client_id);
                cmd.Parameters.AddWithValue("@dept_id", dept_id);
                cmd.Parameters.AddWithValue("@team_id", team_id);
            }
            else
            {
                cmd = new SqlCommand(@"SELECT U.UserId, U.FirstName + ' ' + U.LastName AS FullName 
                                    FROM AllocableUsers U 
                                    WHERE U.DeptId=@dept_id AND U.Active=1 AND
                                    U.UserId NOT IN (SELECT EmployeeId FROM ALOC_TeamMembers WHERE TeamId=@team_id)", cnxn);
                cmd.Parameters.AddWithValue("@dept_id", dept_id);
                cmd.Parameters.AddWithValue("@team_id", team_id);
            }
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds);
            Debug.Assert(ds.Tables.Count == 1);
            ds.Tables[0].Columns.Add(new DataColumn("Title", typeof(String)));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int id = Convert.ToInt32(row["UserId"]);
                cmd = new SqlCommand("dbo.ALOC_GetUserTitle", cnxn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user_id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    row["Title"] = reader["TitleName"];
                reader.Close();
            }
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retval.Add(new ManageTeamResourceRow(Convert.ToInt32(row["UserId"]), row["FullName"].ToString(), row["Title"].ToString()));
            }
            retval.Sort();
            if (cnxn.State == ConnectionState.Open)
                cnxn.Close();

            Debug.WriteLine("obj data source is returning a collection with: " + retval.Count);
            return retval;
        }
    }
}