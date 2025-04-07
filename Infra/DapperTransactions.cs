using Oracle.ManagedDataAccess.Client;

namespace NFK001.Infra
{
    public interface IDapperTransaction
    {
        OracleTransaction Transaction { get; set; }

        void BeginTransaction();
        void Commit();
        void Rollback();
    }

    public class DapperTransaction : IDisposable, IDapperTransaction
    {

        /// <summary>
        /// OracleConnection default
        /// </summary>
        private OracleConnection connection;
        protected OracleConnection Connection
        {
            get
            {
                if (connection is null)
                {
                    connection = new(AppSettings.ConnectionStringMRT);
                    connection.Open();
                }
                return connection;
            }
        }

        /// <summary>
        /// OracleTransaction default
        /// </summary>
        public OracleTransaction Transaction { get; set; }

        /// <summary>
        /// BeginTransaction
        /// </summary>
        public void BeginTransaction() => Transaction = Connection.BeginTransaction();

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit() => Transaction.Commit();

        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback()
        {
            try
            {
                Transaction?.Rollback();
            }
            catch (Exception ex)
            {
                Util.Log($"Erro ao tentar fazer rollback: {ex.Message}", EnTipoLog.Erro);
            }
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
            Connection?.Close();
        }
    }
}
