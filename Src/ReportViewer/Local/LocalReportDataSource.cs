using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Ssrs.ReportViewer.Local
{
    /// <summary>
    /// Class to get a datasource (ConnectionString, Query, ...)
    /// </summary>
    public class LocalReportDataSource
    {
        #region DECLARATIONS

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _connectionString = string.Empty;
        private string _connectionStringOriginal = string.Empty;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a instance of LocalReportDataSource
        /// </summary>
        internal LocalReportDataSource() : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a instance of LocalReportDataSource
        /// </summary>
        /// <param name="username">SQL Server Username to execute the CommandText with the ConnectionString</param>
        /// <param name="password">SQL Server Password to execute the CommandText with the ConnectionString</param>
        internal LocalReportDataSource(string username, string password)
        {
            _username = username;
            _password = password;
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString 
        {
            get
            {
                return _connectionString;
            }
            internal set 
            {
                _connectionStringOriginal = value;

                if (_connectionStringOriginal.EndsWith(";"))
                    _connectionString = _connectionStringOriginal;
                else
                    _connectionString = String.Format("{0};", _connectionStringOriginal);

                if (!this.IsIntegratedSecurity)
                {
                    Dictionary<string, string> items = GetConnectionStringValues(_connectionStringOriginal);

                    // Add "User ID" and "Password" specified by code (in constructor)
                    if (items.Count(i => String.Compare(i.Key, "User ID", true) == 0) <= 0)
                    {
                        _connectionString += String.Format("{0}={1};", "User ID", _username);
                    }

                    if (items.Count(i => String.Compare(i.Key, "Password", true) == 0 && String.Compare(i.Key, "Pwd", true) == 0) <= 0)
                    {
                        _connectionString += String.Format("{0}={1};", "Password", _password);
                    }

                    //StringBuilder result = new StringBuilder();
                    //Dictionary<string, string> items = GetConnectionStringValues(_connectionStringOriginal);

                    //// Concat all connection string items, except Integrated Security
                    //foreach (KeyValuePair<string, string> item in items)
                    //{
                    //    if (String.Compare(item.Key, "Integrated Security", true) != 0 && 
                    //        String.Compare(item.Key, "Trusted_Connection", true) != 0)
                    //    {
                    //        result.AppendFormat("{0}={1};", item.Key, item.Value);
                    //    }
                    //}

                    //// Add "User ID" and "Password" specified by code (in constructor)
                    //if (items.Count(i => String.Compare(i.Key, "User ID", true) == 0) > 0)
                    //{
                    //    result.AppendFormat("{0}={1};", "User ID", _username);                        
                    //}

                    //if (items.Count(i => String.Compare(i.Key, "Password", true) == 0 && String.Compare(i.Key, "Pwd", true) == 0) > 0)
                    //{
                    //    result.AppendFormat("{0}={1};", "Password", _password);                        
                    //}

                    //_connectionString = result.ToString();
                   
               }
            } 
        }

        /// <summary>
        /// Gets True if the connection string is set with integrated security
        /// </summary>
        public bool IsIntegratedSecurity 
        {
            get
            {
                return IncludeIntegratedSecurity(_connectionStringOriginal);
            }
        }

        /// <summary>
        /// Gets the CommandText.
        /// </summary>
        public string CommandText { get; internal set; }

        /// <summary>
        /// Gets the name of the dataSource in the report.
        /// </summary>
        public string DataSourceName { get; internal set; }

        /// <summary>
        /// Gets the name of the dataset in the report.
        /// </summary>
        public string DataSetName { get; internal set; }

        #endregion

        #region PRIVATES

        /// <summary>
        /// Gets the specified ConnectionString cutted in multiples key/value items.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> GetConnectionStringValues(string connectionString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(connectionString))
            {
                string[] items = connectionString.Split(new char[] { ';' });

                foreach (string item in items)
                {
                    string[] keys = item.Split(new char[] { '=' });

                    if (keys.Length > 1)
                    {
                        result.Add(keys[0].Trim(), keys[1].Trim());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets True if the connection string is set with integrated security
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        internal static bool IncludeIntegratedSecurity(string connectionString)
        {
            Dictionary<string, string> items = GetConnectionStringValues(connectionString);

            if (items.Count(i => String.Compare(i.Key, "Integrated Security", true) == 0 && String.Compare(i.Value, "SSPI", true) == 0) > 0)
            {
                return true;
            }

            if (items.Count(i => String.Compare(i.Key, "Trusted_Connection", true) == 0 && String.Compare(i.Value, "True", true) == 0) > 0)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
