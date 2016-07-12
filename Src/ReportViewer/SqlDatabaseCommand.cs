using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Ssrs.ReportViewer
{
    /// <summary>
    /// SQL Command manager, inspired from https://github.com/Apps72/Dev.Data
    /// </summary>
    internal class SqlDatabaseCommand : IDisposable
    {
        private string _connectionString;
        private SqlCommand _command;

        /// <summary>
        /// Initializes a new instance of SqlDatabaseCommand based on the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandtext"></param>
        public SqlDatabaseCommand(string connectionString, string commandtext)
        {
            _connectionString = connectionString;
            _command = new SqlCommand();
        }

        /// <summary>
        /// Initializes a new instance of SqlDatabaseCommand based on the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlDatabaseCommand(string connectionString) : this(connectionString, string.Empty)
        {
        }

        /// <summary>
        /// Gets or sets the query to execute.
        /// </summary>
        public string CommandText
        {
            get
            {
                return _command.CommandText;
            }
            set
            {
                _command.CommandText = value;
            }
        }

        /// <summary>
        /// Gets the <seealso cref="System.Data.SqlClient.SqlParameterCollection"/>
        /// The parameters of the Transact-SQL statement or stored procedure.
        /// </summary>
        public SqlParameterCollection Parameters { get { return _command.Parameters; } }

        /// <summary>
        /// Execute the <see cref="CommandText"/> query and return a <seealso cref="System.Data.DataTable"/>.
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteTable()
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                _command.Connection = connection;

                using (SqlDataAdapter adapter = new SqlDataAdapter(_command))
                {
                    adapter.Fill(table);
                }

                connection.Close();
            }
            return table;
        }

        /// <summary>
        /// Dispose all managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose all managed or unmanaged resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _command.Dispose();
            }
        }

        /// <summary>
        /// Dispose all unmanaged resources
        /// </summary>
        ~SqlDatabaseCommand()
        {
            Dispose(false);
        }

    }
}
