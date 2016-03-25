using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XClip.Repositories
{
    public class RepositoryBase
    {
        private string _connectionString;

        public RepositoryBase()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ResumeDb"].ConnectionString;
        }

        /// <summary>
        /// Overloaded constructor accepting a connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public RepositoryBase(string connectionString)
        {
            this._connectionString = connectionString;
        }

        /// <summary>
        /// The connection string which this class will use to perform its operations against
        /// </summary>
        public string ConnectionString
        {
            get { return this._connectionString; }
            set { this._connectionString = value; }
        }

        public DbParameter AddParameter(DbCommand cmd, string name, DbType type, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Value = value;

            return param;
        }

        public T ExecuteScalar<T>(string sqlStatement, IList<SqlParameter> paramList)
        {
            using (var connection = new SqlConnection(this._connectionString))
            {
                using (var cmdSql = new SqlCommand(sqlStatement, connection))
                {
                    cmdSql.CommandTimeout = 3000;
                    cmdSql.CommandType = CommandType.Text;

                    if (paramList != null)
                        foreach (var p in paramList) { cmdSql.Parameters.Add(p); }

                    connection.Open();
                    var x = cmdSql.ExecuteScalar();

                    if (x == null)
                        return default(T);

                    return (T)x;
                }
            }
        }

        /// <summary>
        /// Executes a simple in-line SQL statement that uses SQL parameters
        /// FxCop violation suppressed because the sqlStatement is parameterized
        /// </summary>
        /// <param name="sqlStatement">the SQL statement to execute</param>
        /// <param name="paramList">list of SQL parameters</param>
        /// <returns>true if successful; false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public bool ExecuteInLineSql(string sqlStatement, IList<SqlParameter> paramList)
        {
            using (var connection = new SqlConnection(this._connectionString))
            {
                using (var cmdSql = new SqlCommand(sqlStatement, connection))
                {
                    cmdSql.CommandTimeout = 3000;
                    cmdSql.CommandType = CommandType.Text;

                    if (paramList != null)
                        foreach (var p in paramList) { cmdSql.Parameters.Add(p); }

                    connection.Open();
                    cmdSql.ExecuteNonQuery();

                    return true;
                }
            }
        }

        public int ExecuteIdentity(string sql, IList<SqlParameter> paramList)
        {

            using (var connection = new SqlConnection(this._connectionString))
            {
                using (var cmdSql = new SqlCommand(sql, connection))
                {
                    cmdSql.CommandTimeout = 3000;
                    cmdSql.CommandType = CommandType.Text;

                    if (paramList != null)
                        foreach (var p in paramList) { cmdSql.Parameters.Add(p); }

                    connection.Open();

                    var val = cmdSql.ExecuteScalar();

                    int id;
                    int.TryParse(val.ToString(), out id);

                    return id;
                }
            }
        }

        /// <summary>
        /// Executes a stored procedure
        /// </summary>
        /// <param name="storedProcName">the name of the stored procedure</param>
        /// <param name="paramList">list of SQL parameters</param>
        /// <returns>true if successful; false otherwise</returns>
        public bool ExecuteSql(string storedProcName, List<SqlParameter> paramList)
        {

            using (var connection = new SqlConnection(this.ConnectionString))
            {

                using (var cmdSql = new SqlCommand(storedProcName, connection))
                {

                    cmdSql.CommandTimeout = 3000;
                    cmdSql.CommandType = CommandType.StoredProcedure;

                    foreach (var p in paramList) { cmdSql.Parameters.Add(p); }

                    connection.Open();
                    cmdSql.ExecuteNonQuery();

                    return true;
                }

            }

        }
    }
}
