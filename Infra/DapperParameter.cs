using Oracle.ManagedDataAccess.Client;

namespace NFK001.Infra
{
    public class DapperParameter
    {
        public DapperParameter() { }

        public DapperParameter(string parameterName, object parameterValue, OracleParameterStatus[] parameterStatus)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
            ParameterStatus = parameterStatus;
        }

        public DapperParameter(string parameterName, object parameterValue, OracleParameterStatus[] parameterStatus, OracleDbType? oracleDbType)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
            ParameterStatus = parameterStatus;
            OracleDbType = oracleDbType;
        }

        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public OracleParameterStatus[] ParameterStatus { get; set; }
        public OracleDbType? OracleDbType { get; set; }
    }
}
