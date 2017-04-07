using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace KL7A.DataAccess
{
    internal class Database : IDisposable
    {
        private string _connectionString;
        private SqlConnection _cn;
        private SqlTransaction _tran;

        private Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            if (_cn == null)
            {
                _cn = new SqlConnection();
                _cn.ConnectionString = _connectionString;
                _cn.Open();
            }

            if (_cn.State != System.Data.ConnectionState.Open)
            {
                _cn.Open();
            }

            return _cn;
        }
        private SqlCommand GetCommand(CommandType ct, string sql, params SqlParameter[] parameters)
        {
            SqlCommand cmd = GetConnection().CreateCommand();

            cmd.CommandType = ct;
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(parameters);

            if (_tran != null)
            {
                cmd.Transaction = _tran;
            }

            return cmd;
        }

        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = GetCommand(CommandType.Text, sql, parameters))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = GetCommand(CommandType.Text, sql, parameters))
            {
                return cmd.ExecuteScalar();
            }
        }
        public List<T> ExecuteReader<T>(string sql, Func<SqlDataReader, List<T>> factoryMethod, params SqlParameter[] parameters)
        {
            List<T> result = new List<T>();
            if (factoryMethod == null) return result;
            using (SqlCommand cmd = GetCommand(CommandType.Text, sql, parameters))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    return factoryMethod(rdr);
                }
            }
        }

        public void BeginTransaction()
        {
            if (_tran == null)
            {
                _tran = GetConnection().BeginTransaction();
            }
        }
        public void CommitTransaction()
        {
            if (_tran != null)
            {
                _tran.Commit();

                _tran.Dispose();
                _tran = null;
            }
        }
        public void RollbackTransaction()
        {
            if (_tran != null)
            {
                _tran.Rollback();

                _tran.Dispose();
                _tran = null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_tran != null)
                    {
                        _tran.Rollback();

                        _tran.Dispose();
                        _tran = null;
                    }

                    if (_cn != null)
                    {
                        if (_cn.State == ConnectionState.Open)
                        {
                            _cn.Close();
                        }
                        _cn.Dispose();
                        _cn = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Database() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public static Database Get()
        {
            var con = ConfigurationManager.ConnectionStrings["Database"];
            if (con != null)
            {
                return new Database(con.ConnectionString);
            }

            return new Database("Server=(local);Database=Settings;Integrated Security=SSPI;MultipleActiveResultSets=true");
        }

    }
}
