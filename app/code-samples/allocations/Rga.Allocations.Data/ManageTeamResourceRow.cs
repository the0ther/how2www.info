using System;
using System.Data;
using System.Configuration;

namespace RGA.Allocations.Data
{
    /// <summary>
    /// Summary description for ManageTeamResourceRow
    /// </summary>
    public class ManageTeamResourceRow : IComparable
    {
        private int m_EmpId;
        private string m_FullName;
        private string m_Title;

        public ManageTeamResourceRow()
        {
        }

        public ManageTeamResourceRow(int id, string name, string title)
        {
            this.m_EmpId = id;
            this.m_FullName = name;
            this.m_Title = title;
        }

        public int EmpId
        {
            get { return m_EmpId; }
            set { m_EmpId = value; }
        }

        public string FullName
        {
            get { return m_FullName; }
            set { m_FullName = value; }
        }

        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        public int CompareTo(object o1)
        {
            ManageTeamResourceRow otherRow = (ManageTeamResourceRow)o1;
            return this.FullName.CompareTo(otherRow.FullName);
        }
    }
}