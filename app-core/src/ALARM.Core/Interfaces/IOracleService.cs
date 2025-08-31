namespace ALARM.Core.Interfaces;

/// <summary>
/// Abstraction for Oracle database operations
/// </summary>
public interface IOracleService
{
    /// <summary>
    /// Gets a connection factory for creating database connections
    /// </summary>
    IOracleConnectionFactory ConnectionFactory { get; }
    
    /// <summary>
    /// Gets a command builder for creating parameterized commands
    /// </summary>
    IOracleCommandBuilder CommandBuilder { get; }
    
    /// <summary>
    /// Gets a data service for high-level data operations
    /// </summary>
    IOracleDataService DataService { get; }
    
    /// <summary>
    /// Tests database connectivity
    /// </summary>
    Task<ConnectionTestResult> TestConnectionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets database server information
    /// </summary>
    Task<DatabaseInfo> GetDatabaseInfoAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a health check on the database
    /// </summary>
    Task<HealthCheckResult> PerformHealthCheckAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Factory for creating Oracle database connections
/// </summary>
public interface IOracleConnectionFactory
{
    /// <summary>
    /// Creates a new database connection
    /// </summary>
    Task<IOracleConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a connection with a specific connection string
    /// </summary>
    Task<IOracleConnection> CreateConnectionAsync(string connectionString, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the default connection string (without sensitive data)
    /// </summary>
    string GetConnectionStringInfo();
    
    /// <summary>
    /// Validates a connection string
    /// </summary>
    bool ValidateConnectionString(string connectionString);
}

/// <summary>
/// Abstraction for Oracle database connections
/// </summary>
public interface IOracleConnection : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Connection string (sensitive data masked)
    /// </summary>
    string ConnectionStringInfo { get; }
    
    /// <summary>
    /// Current connection state
    /// </summary>
    ConnectionState State { get; }
    
    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    int ConnectionTimeout { get; }
    
    /// <summary>
    /// Database name/service
    /// </summary>
    string Database { get; }
    
    /// <summary>
    /// Server version
    /// </summary>
    string ServerVersion { get; }
    
    /// <summary>
    /// Opens the connection
    /// </summary>
    Task OpenAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Closes the connection
    /// </summary>
    Task CloseAsync();
    
    /// <summary>
    /// Begins a new transaction
    /// </summary>
    Task<IOracleTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a command for this connection
    /// </summary>
    IOracleCommand CreateCommand();
    
    /// <summary>
    /// Creates a command with SQL text
    /// </summary>
    IOracleCommand CreateCommand(string sql);
    
    /// <summary>
    /// Tests the connection
    /// </summary>
    Task<bool> TestAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstraction for Oracle database transactions
/// </summary>
public interface IOracleTransaction : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Connection associated with this transaction
    /// </summary>
    IOracleConnection Connection { get; }
    
    /// <summary>
    /// Isolation level of the transaction
    /// </summary>
    IsolationLevel IsolationLevel { get; }
    
    /// <summary>
    /// Commits the transaction
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back the transaction
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a savepoint
    /// </summary>
    Task CreateSavepointAsync(string savepointName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back to a savepoint
    /// </summary>
    Task RollbackToSavepointAsync(string savepointName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstraction for Oracle database commands
/// </summary>
public interface IOracleCommand : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// SQL command text
    /// </summary>
    string CommandText { get; set; }
    
    /// <summary>
    /// Command type (Text, StoredProcedure, etc.)
    /// </summary>
    CommandType CommandType { get; set; }
    
    /// <summary>
    /// Command timeout in seconds
    /// </summary>
    int CommandTimeout { get; set; }
    
    /// <summary>
    /// Connection for this command
    /// </summary>
    IOracleConnection Connection { get; set; }
    
    /// <summary>
    /// Transaction for this command
    /// </summary>
    IOracleTransaction? Transaction { get; set; }
    
    /// <summary>
    /// Command parameters
    /// </summary>
    IOracleParameterCollection Parameters { get; }
    
    /// <summary>
    /// Executes the command and returns the number of affected rows
    /// </summary>
    Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the command and returns a scalar value
    /// </summary>
    Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the command and returns a data reader
    /// </summary>
    Task<IOracleDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the command and returns a data reader with specific behavior
    /// </summary>
    Task<IOracleDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Prepares the command for execution
    /// </summary>
    Task PrepareAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancels command execution
    /// </summary>
    void Cancel();
}

/// <summary>
/// Collection of Oracle command parameters
/// </summary>
public interface IOracleParameterCollection : IEnumerable<IOracleParameter>
{
    /// <summary>
    /// Number of parameters
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// Gets parameter by index
    /// </summary>
    IOracleParameter this[int index] { get; set; }
    
    /// <summary>
    /// Gets parameter by name
    /// </summary>
    IOracleParameter this[string parameterName] { get; set; }
    
    /// <summary>
    /// Adds a parameter
    /// </summary>
    IOracleParameter Add(IOracleParameter parameter);
    
    /// <summary>
    /// Adds a parameter with name and value
    /// </summary>
    IOracleParameter Add(string parameterName, object? value);
    
    /// <summary>
    /// Adds a parameter with name, type, and value
    /// </summary>
    IOracleParameter Add(string parameterName, OracleDbType dbType, object? value);
    
    /// <summary>
    /// Removes a parameter
    /// </summary>
    bool Remove(IOracleParameter parameter);
    
    /// <summary>
    /// Removes a parameter by name
    /// </summary>
    bool Remove(string parameterName);
    
    /// <summary>
    /// Clears all parameters
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Checks if parameter exists
    /// </summary>
    bool Contains(string parameterName);
}

/// <summary>
/// Oracle command parameter
/// </summary>
public interface IOracleParameter
{
    /// <summary>
    /// Parameter name
    /// </summary>
    string ParameterName { get; set; }
    
    /// <summary>
    /// Parameter value
    /// </summary>
    object? Value { get; set; }
    
    /// <summary>
    /// Oracle database type
    /// </summary>
    OracleDbType DbType { get; set; }
    
    /// <summary>
    /// Parameter direction (Input, Output, etc.)
    /// </summary>
    ParameterDirection Direction { get; set; }
    
    /// <summary>
    /// Parameter size
    /// </summary>
    int Size { get; set; }
    
    /// <summary>
    /// Precision for numeric types
    /// </summary>
    byte Precision { get; set; }
    
    /// <summary>
    /// Scale for numeric types
    /// </summary>
    byte Scale { get; set; }
    
    /// <summary>
    /// Whether parameter can be null
    /// </summary>
    bool IsNullable { get; set; }
}

/// <summary>
/// Oracle data reader abstraction
/// </summary>
public interface IOracleDataReader : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Number of fields in the current record
    /// </summary>
    int FieldCount { get; }
    
    /// <summary>
    /// Whether reader has rows
    /// </summary>
    bool HasRows { get; }
    
    /// <summary>
    /// Whether reader is closed
    /// </summary>
    bool IsClosed { get; }
    
    /// <summary>
    /// Number of records affected
    /// </summary>
    int RecordsAffected { get; }
    
    /// <summary>
    /// Gets value by column index
    /// </summary>
    object this[int index] { get; }
    
    /// <summary>
    /// Gets value by column name
    /// </summary>
    object this[string name] { get; }
    
    /// <summary>
    /// Advances to the next record
    /// </summary>
    Task<bool> ReadAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Advances to the next result set
    /// </summary>
    Task<bool> NextResultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets column name by index
    /// </summary>
    string GetName(int index);
    
    /// <summary>
    /// Gets column index by name
    /// </summary>
    int GetOrdinal(string name);
    
    /// <summary>
    /// Gets data type of column
    /// </summary>
    Type GetFieldType(int index);
    
    /// <summary>
    /// Gets Oracle-specific data type
    /// </summary>
    OracleDbType GetOracleDbType(int index);
    
    /// <summary>
    /// Gets value as specific type
    /// </summary>
    T GetFieldValue<T>(int index);
    
    /// <summary>
    /// Gets value as specific type asynchronously
    /// </summary>
    Task<T> GetFieldValueAsync<T>(int index, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if column value is null
    /// </summary>
    bool IsDBNull(int index);
    
    /// <summary>
    /// Checks if column value is null asynchronously
    /// </summary>
    Task<bool> IsDBNullAsync(int index, CancellationToken cancellationToken = default);
}

/// <summary>
/// Oracle database types enumeration
/// </summary>
public enum OracleDbType
{
    Varchar2,
    Number,
    Date,
    Timestamp,
    Clob,
    Blob,
    Char,
    NVarchar2,
    NChar,
    NClob,
    Raw,
    LongRaw,
    BinaryFloat,
    BinaryDouble,
    Boolean,
    IntervalYearToMonth,
    IntervalDayToSecond,
    TimestampWithTimeZone,
    TimestampWithLocalTimeZone
}

/// <summary>
/// Connection state enumeration
/// </summary>
public enum ConnectionState
{
    Closed,
    Open,
    Connecting,
    Executing,
    Fetching,
    Broken
}

/// <summary>
/// Command type enumeration
/// </summary>
public enum CommandType
{
    Text,
    StoredProcedure,
    TableDirect
}

/// <summary>
/// Command behavior enumeration
/// </summary>
public enum CommandBehavior
{
    Default,
    SingleResult,
    SchemaOnly,
    KeyInfo,
    SingleRow,
    SequentialAccess,
    CloseConnection
}

/// <summary>
/// Parameter direction enumeration
/// </summary>
public enum ParameterDirection
{
    Input,
    Output,
    InputOutput,
    ReturnValue
}

/// <summary>
/// Transaction isolation level enumeration
/// </summary>
public enum IsolationLevel
{
    Unspecified,
    ReadUncommitted,
    ReadCommitted,
    RepeatableRead,
    Serializable,
    Snapshot
}

/// <summary>
/// Connection test result
/// </summary>
public record ConnectionTestResult(
    bool IsSuccessful,
    TimeSpan Duration,
    string? ErrorMessage,
    DateTime TestedAt
);

/// <summary>
/// Database information
/// </summary>
public record DatabaseInfo(
    string ServerVersion,
    string DatabaseName,
    string ServiceName,
    string CharacterSet,
    string NationalCharacterSet,
    DateTime ServerTime,
    TimeSpan Uptime
);

/// <summary>
/// Health check result
/// </summary>
public record HealthCheckResult(
    bool IsHealthy,
    Dictionary<string, object> Metrics,
    List<string> Issues,
    DateTime CheckedAt
);
