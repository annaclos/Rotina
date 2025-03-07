using Dapper;
using NFK001.Infra;
using NFK001;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NFK001.Infra
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext()
        {
            _connectionString = AppSettings.ConnectionStringMRT;
        }

        /// <summary>
        /// Create connection
        /// </summary>
        /// <returns>OracleConnection</returns>
        public OracleConnection CreateConnection() => new(_connectionString);

        /// <summary>
        /// Query parameters
        /// </summary>
        public DynamicParameters Parameters { get; set; } = new();

        /// <summary>
        /// Bulk parameters
        /// </summary>
        public List<DapperParameter> ArrayParameters { get; set; } = [];

        /// <summary>
        /// Execute query async with manual parameters
        /// </summary>
        /// <typeparam name="T">Generic type return</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>IEnumerable of T</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, int? commandTimeout = null)
        {
            try
            {
                using OracleConnection connection = CreateConnection();
                Parameters.RemoveUnused = true;
                IEnumerable<T> result = await connection.QueryAsync<T>(query, Parameters, commandTimeout: commandTimeout);
                return result;
            }
            finally { Parameters = new(); }
        }

        /// <summary>
        /// Execute query async with manual parameters and transaction
        /// </summary>
        /// <typeparam name="T">Generic type return</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="transaction">Oracle transaction</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>IEnumerable of T</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, OracleTransaction transaction, int? commandTimeout = null)
        {
            try
            {
                Parameters.RemoveUnused = true;
                IEnumerable<T> result = await transaction.Connection.QueryAsync<T>(query, Parameters, commandTimeout: commandTimeout, transaction: transaction);
                return result;
            }
            finally { Parameters = new(); }
        }

        /// <summary>
        /// Execute query async with model parameters
        /// </summary>
        /// <typeparam name="T">Generic type return</typeparam>
        /// <typeparam name="P">Generic type model param</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="parameters">Model parameters</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>IEnumerable of T</returns>
        public async Task<IEnumerable<T>> QueryAsync<T, P>(string query, P parameters, int? commandTimeout = null)
        {
            using OracleConnection connection = CreateConnection();
            return await connection.QueryAsync<T>(query, parameters, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Execute query async with model parameters
        /// </summary>
        /// <typeparam name="T">Generic type return</typeparam>
        /// <typeparam name="P">Generic type model param</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="parameters">Model parameters</param>
        /// <param name="transaction">Oracle transaction</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>IEnumerable of T</returns>
        public async Task<IEnumerable<T>> QueryAsync<T, P>(string query, P parameters, OracleTransaction transaction, int? commandTimeout = null)
            => await transaction.Connection.QueryAsync<T>(query, parameters, commandTimeout: commandTimeout, transaction: transaction);

        /// <summary>
        /// Execute command async
        /// </summary>
        /// <param name="query">String sql query</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows</returns>
        public async Task<int> ExecuteAsync(string query, int? commandTimeout = null)
        {
            try
            {
                using OracleConnection connection = CreateConnection();
                Parameters.RemoveUnused = true;
                int result = await connection.ExecuteAsync(query, Parameters, commandTimeout: commandTimeout);
                return result;
            }
            finally { Parameters = new(); }
        }

        /// <summary>
        /// Execute command async
        /// </summary>
        /// <param name="query">String sql query</param>
        /// <param name="transaction">Oracle transaction</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows</returns>
        public async Task<int> ExecuteAsync(string query, OracleTransaction transaction, int? commandTimeout = null)
        {
            try
            {
                Parameters.RemoveUnused = true;
                int result = await transaction.Connection.ExecuteAsync(query, Parameters, commandTimeout: commandTimeout, transaction: transaction);
                return result;
            }
            finally { Parameters = new(); }
        }

        /// <summary>
        /// Execute command async
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="model">T model</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows</returns>
        public async Task<int> ExecuteAsync<T>(string query, T model, int? commandTimeout = null)
        {
            using OracleConnection connection = CreateConnection();
            return await connection.ExecuteAsync(query, model, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Execute command async
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="model">T model</param>
        /// <param name="transaction">Oracle transaction</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows</returns>
        public async Task<int> ExecuteAsync<T>(string query, T model, OracleTransaction transaction, int? commandTimeout = null)
            => await transaction.Connection.ExecuteAsync(query, model, commandTimeout: commandTimeout, transaction: transaction);

        /// <summary>
        /// Execute command in bulk async
        /// </summary>
        /// <param name="query">String sql query</param>
        /// <param name="bindCount">Count of list parameters</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns></returns>
        public async Task<int> ExecuteBulkAsync(string query, int bindCount, int? commandTimeout = null)
        {
            try
            {
                using OracleConnection connection = CreateConnection();
                int affectedRows = await ExecuteBulkAsAsyncAction(connection, query, bindCount, ArrayParameters, commandTimeout);
                return affectedRows;
            }
            finally { ArrayParameters.Clear(); }
        }

        /// <summary>
        /// Execute command in bulk async
        /// </summary>
        /// <param name="query">String sql query</param>
        /// <param name="bindCount">Count of list parameters</param>
        /// <param name="transaction">Oracle transaction</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns></returns>
        public async Task<int> ExecuteBulkAsync(string query, int bindCount, OracleTransaction transaction, int? commandTimeout = null)
        {
            try
            {
                int affectedRows = await ExecuteBulkAsAsyncAction(transaction.Connection, query, bindCount, ArrayParameters, commandTimeout);
                return affectedRows;
            }
            finally { ArrayParameters.Clear(); }
        }

        /// <summary>
        /// Execute command in bulk async
        /// </summary>
        /// <typeparam name="P">Generic parameter</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="param">Genetic list parameters</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows count</returns>
        public async Task<int> ExecuteBulkAsync<P>(string query, List<P> param, int? commandTimeout = null)
        {
            using OracleConnection connection = CreateConnection();
            return await ExecuteBulkAsyncAction(connection, query, param, commandTimeout);
        }

        /// <summary>
        /// Execute command in bulk async
        /// </summary>
        /// <typeparam name="P">Generic parameter</typeparam>
        /// <param name="query">String sql query</param>
        /// <param name="param">Genetic list parameters</param>
        /// <param name="transaction">Oracle transaction</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows count</returns>
        public async Task<int> ExecuteBulkAsync<P>(string query, List<P> param, OracleTransaction transaction, int? commandTimeout = null)
            => await ExecuteBulkAsyncAction(transaction.Connection, query, param, commandTimeout);

        /// <summary>
        /// Execute command in bulk async
        /// </summary>
        /// <typeparam name="P">Generic parameter</typeparam>
        /// <param name="connection">Oracle connection</param>
        /// <param name="query">String sql query</param>
        /// <param name="param">Genetic list parameters</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows count</returns>
        private async Task<int> ExecuteBulkAsyncAction<P>(OracleConnection connection, string query, List<P> param, int? commandTimeout)
        {
            Regex regex = new(@"(?<param>:\w*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(query);
            Type type = param.GetType().GetGenericArguments().Single();
            List<DapperParameter> parameters = [];
            foreach (Match match in matches)
                if (type.GetProperty(match.Value.Remove(0, 1)) is PropertyInfo prop && prop is not null)
                {
                    if (Nullable.GetUnderlyingType(prop.PropertyType) is null)
                        parameters.Add(new(prop.Name,
                            param.SelectArray(p => Convert.ChangeType(p.GetType().GetProperty(prop.Name).GetValue(p, null), prop.PropertyType)),
                            null,
                            prop.GetOracleType()));
                    else
                        parameters.Add(new(prop.Name,
                            param.SelectArray(p => p.GetType().GetProperty(prop.Name).GetValue(p, null) is var value && value is not null ? Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType)) : null),
                            param.SelectOracleStatus(p => p.GetType().GetProperty(prop.Name).GetValue(p, null) is var value && value is not null ? Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType)) : null),
                            prop.GetOracleType()));
                }

            return await ExecuteBulkAsAsyncAction(connection, query, param.Count, parameters, commandTimeout);
        }

        /// <summary>
        /// Execute command in bulk async
        /// </summary>
        /// <param name="connection">Oracle connection</param>
        /// <param name="query">String sql query</param>
        /// <param name="bindCount">Count of list parameters</param>
        /// <param name="parameters">Dapper parameters</param>
        /// <param name="commandTimeout">Optional timeout</param>
        /// <returns>Affected rows count</returns>
        private async Task<int> ExecuteBulkAsAsyncAction(OracleConnection connection, string query, int bindCount, List<DapperParameter> parameters, int? commandTimeout)
        {
            if (connection.State is not ConnectionState.Open)
                connection.Open();
            OracleCommand command = connection.CreateCommand();
            command.ArrayBindCount = bindCount;
            command.BindByName = false;
            command.CommandText = query;
            command.CommandTimeout = commandTimeout.GetValueOrDefault();
            parameters.ForEach(param => command.AddParameterArray(param.ParameterName, param.ParameterValue, param.OracleDbType, param.ParameterStatus));
            int result = await command.ExecuteNonQueryAsync();
            return result;
        }
    }

    public static class DapperExtensions
    {
        /// <summary>
        /// Add Parameter
        /// </summary>
        /// <param name="context">DapperContext</param>
        /// <param name="name">Parameter Name</param>
        /// <param name="value">Parameter Value</param>
        /// <returns>DapperContext</returns>
        public static DapperContext AddParameter(this DapperContext context, string name, object value)
        {
            context.Parameters.Add(name, value);
            return context;
        }

        /// <summary>
        /// Add Parameter
        /// </summary>
        /// <param name="context">DapperContext</param>
        /// <param name="name">Parameter Name</param>
        /// <param name="value">Parameter Value</param>
        /// <returns>DapperContext</returns>
        public static DapperContext AddParameterArray(this DapperContext context, string name, object value, OracleParameterStatus[] status = null)
        {
            context.ArrayParameters.Add(new(name, value, status));
            return context;
        }

        /// <summary>
        /// Adiciona um array de parâmetros no comando Oracle
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle</param>
        /// <param name="paramName">Nome do parâmetro</param>
        /// <param name="paramValue">Valor do parâmetro</param>
        /// <returns>Commando Oracle</returns>
        public static OracleCommand AddParameterArray(this OracleCommand oraCommand, string paramName, object paramValue)
            => oraCommand.AddParameterArrayAction(paramName, paramValue, null, null);

        /// <summary>
        /// Adiciona um array de parâmetros no comando Oracle
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle</param>
        /// <param name="paramName">Nome do parâmetro</param>
        /// <param name="paramValue">Valor do parâmetro</param>
        /// <param name="statusValue">Status do parâmetro</param>
        /// <returns>Commando Oracle</returns>
        public static OracleCommand AddParameterArray(this OracleCommand oraCommand, string paramName, object paramValue, OracleParameterStatus[] statusValue)
            => oraCommand.AddParameterArrayAction(paramName, paramValue, null, statusValue);

        /// <summary>
        /// Adiciona um array de parâmetros no comando Oracle
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle</param>
        /// <param name="paramName">Nome do parâmetro</param>
        /// <param name="paramValue">Valor do parâmetro</param>
        /// <param name="oracleDbType">Tipo do parâmetro</param>
        /// <param name="statusValue">Status do parâmetro</param>
        /// <returns>Commando Oracle</returns>
        public static OracleCommand AddParameterArray(this OracleCommand oraCommand, string paramName, object paramValue, OracleDbType? oracleDbType, OracleParameterStatus[] statusValue)
            => oraCommand.AddParameterArrayAction(paramName, paramValue, oracleDbType, statusValue);

        /// <summary>
        /// Adiciona um array de parâmetros no comando Oracle
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle</param>
        /// <param name="paramName">Nome do parâmetro</param>
        /// <param name="paramValue">Valor do parâmetro</param>
        /// <param name="oracleDbType">Tipo do parâmetro</param>
        /// <param name="statusValue">Status do parâmetro</param>
        /// <returns>Commando Oracle</returns>
        public static OracleCommand AddParameterArrayAction(this OracleCommand oraCommand, string paramName, object paramValue, OracleDbType? oracleDbType, OracleParameterStatus[] statusValue)
        {
            OracleParameter param = oraCommand.CreateParameter();
            if (paramValue is null)
                param.Value = DBNull.Value;
            else
            {
                if (statusValue is not null)
                    param.ArrayBindStatus = statusValue;
                if (oracleDbType is not null)
                    param.OracleDbType = oracleDbType.Value;
                param.Value = paramValue;
            }
            oraCommand.Parameters.Add(param);
            return oraCommand;
        }

        /// <summary>
        /// Retornar um enumerable convertido para array primitivo
        /// </summary>
        /// <typeparam name="TSource">TSource</typeparam>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="source">source</param>
        /// <param name="selector">selector</param>
        /// <returns>Array</returns>
        public static TResult[] SelectArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToArray();
        }

        /// <summary>
        /// Retornar um enumerable convertido em OracleParameterStatus
        /// </summary>
        /// <typeparam name="TSource">TSource</typeparam>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="source">source</param>
        /// <param name="selector">selector</param>
        /// <returns>OracleParameterStatus Array</returns>
        public static OracleParameterStatus[] SelectOracleStatus<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            List<OracleParameterStatus> oracleParameters = [];
            foreach (TSource item in source)
            {
                oracleParameters.Add(
                    selector(item) is null
                    ? OracleParameterStatus.NullInsert
                    : OracleParameterStatus.Success
                );
            }
            return [.. oracleParameters];
        }

        /// <summary>
        /// Convert a PropertyInfo Type to OracleDbType
        /// </summary>
        /// <param name="prop">PropertyInfo</param>
        /// <returns>OracleDbType</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static OracleDbType GetOracleType(this PropertyInfo prop)
        {
            Type type = Nullable.GetUnderlyingType(prop.PropertyType) is null ? prop.PropertyType : Nullable.GetUnderlyingType(prop.PropertyType);
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Boolean => OracleDbType.Boolean,
                TypeCode.Byte or TypeCode.SByte => OracleDbType.Byte,
                TypeCode.Char => OracleDbType.Char,
                TypeCode.DateTime => OracleDbType.Date,
                TypeCode.Decimal => OracleDbType.Decimal,
                TypeCode.Double => OracleDbType.Double,
                TypeCode.Int16 or TypeCode.UInt16 => OracleDbType.Int16,
                TypeCode.Int32 or TypeCode.UInt32 => OracleDbType.Int32,
                TypeCode.Int64 or TypeCode.UInt64 => OracleDbType.Int64,
                TypeCode.Single => OracleDbType.Single,
                TypeCode.String => OracleDbType.Varchar2,
                TypeCode.Empty => throw new NullReferenceException("The target type is null."),
                TypeCode.Object => throw new InvalidCastException($"Cannot convert from to {prop.PropertyType.Name}."),
                _ => throw new InvalidCastException("Conversion not supported.")
            };
        }
    }
}
