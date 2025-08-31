namespace ALARM.Core.Interfaces;

/// <summary>
/// High-level Oracle data service abstraction
/// </summary>
public interface IOracleDataService
{
    /// <summary>
    /// Executes a query and returns results as a list of dynamic objects
    /// </summary>
    Task<IList<dynamic>> QueryAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a query and returns results as strongly-typed objects
    /// </summary>
    Task<IList<T>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a query and returns the first result or default
    /// </summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a query and returns a single result
    /// </summary>
    Task<T> QuerySingleAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a scalar query and returns the result
    /// </summary>
    Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a non-query command and returns affected rows
    /// </summary>
    Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes multiple commands in a transaction
    /// </summary>
    Task<int> ExecuteTransactionAsync(IEnumerable<SqlCommand> commands, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Inserts an entity and returns the number of affected rows
    /// </summary>
    Task<int> InsertAsync<T>(T entity, string? tableName = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an entity and returns the number of affected rows
    /// </summary>
    Task<int> UpdateAsync<T>(T entity, string? tableName = null, object? whereConditions = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes records matching the conditions
    /// </summary>
    Task<int> DeleteAsync<T>(object whereConditions, string? tableName = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Bulk inserts multiple entities
    /// </summary>
    Task<int> BulkInsertAsync<T>(IEnumerable<T> entities, string? tableName = null, int batchSize = 1000, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a stored procedure
    /// </summary>
    Task<StoredProcedureResult> ExecuteStoredProcedureAsync(string procedureName, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a stored procedure and returns results as strongly-typed objects
    /// </summary>
    Task<StoredProcedureResult<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets table metadata
    /// </summary>
    Task<TableMetadata> GetTableMetadataAsync(string tableName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a table exists
    /// </summary>
    Task<bool> TableExistsAsync(string tableName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets database schema information
    /// </summary>
    Task<SchemaInfo> GetSchemaInfoAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Oracle command builder for creating parameterized queries
/// </summary>
public interface IOracleCommandBuilder
{
    /// <summary>
    /// Creates a SELECT command builder
    /// </summary>
    ISelectCommandBuilder Select(params string[] columns);
    
    /// <summary>
    /// Creates an INSERT command builder
    /// </summary>
    IInsertCommandBuilder InsertInto(string tableName);
    
    /// <summary>
    /// Creates an UPDATE command builder
    /// </summary>
    IUpdateCommandBuilder Update(string tableName);
    
    /// <summary>
    /// Creates a DELETE command builder
    /// </summary>
    IDeleteCommandBuilder DeleteFrom(string tableName);
    
    /// <summary>
    /// Creates a raw SQL command
    /// </summary>
    ISqlCommand Raw(string sql);
    
    /// <summary>
    /// Creates a stored procedure command
    /// </summary>
    IStoredProcedureCommand StoredProcedure(string procedureName);
}

/// <summary>
/// SELECT command builder
/// </summary>
public interface ISelectCommandBuilder
{
    ISelectCommandBuilder From(string tableName);
    ISelectCommandBuilder Join(string tableName, string condition);
    ISelectCommandBuilder LeftJoin(string tableName, string condition);
    ISelectCommandBuilder RightJoin(string tableName, string condition);
    ISelectCommandBuilder InnerJoin(string tableName, string condition);
    ISelectCommandBuilder Where(string condition);
    ISelectCommandBuilder WhereEquals(string column, object value);
    ISelectCommandBuilder WhereIn(string column, IEnumerable<object> values);
    ISelectCommandBuilder WhereBetween(string column, object minValue, object maxValue);
    ISelectCommandBuilder WhereNull(string column);
    ISelectCommandBuilder WhereNotNull(string column);
    ISelectCommandBuilder OrderBy(string column, SortDirection direction = SortDirection.Ascending);
    ISelectCommandBuilder GroupBy(params string[] columns);
    ISelectCommandBuilder Having(string condition);
    ISelectCommandBuilder Limit(int count);
    ISelectCommandBuilder Offset(int count);
    ISqlCommand Build();
}

/// <summary>
/// INSERT command builder
/// </summary>
public interface IInsertCommandBuilder
{
    IInsertCommandBuilder Values(object values);
    IInsertCommandBuilder Value(string column, object value);
    IInsertCommandBuilder Select(ISelectCommandBuilder selectBuilder);
    ISqlCommand Build();
}

/// <summary>
/// UPDATE command builder
/// </summary>
public interface IUpdateCommandBuilder
{
    IUpdateCommandBuilder Set(string column, object value);
    IUpdateCommandBuilder Set(object values);
    IUpdateCommandBuilder Where(string condition);
    IUpdateCommandBuilder WhereEquals(string column, object value);
    ISqlCommand Build();
}

/// <summary>
/// DELETE command builder
/// </summary>
public interface IDeleteCommandBuilder
{
    IDeleteCommandBuilder Where(string condition);
    IDeleteCommandBuilder WhereEquals(string column, object value);
    IDeleteCommandBuilder WhereIn(string column, IEnumerable<object> values);
    ISqlCommand Build();
}

/// <summary>
/// SQL command interface
/// </summary>
public interface ISqlCommand
{
    string CommandText { get; }
    IDictionary<string, object?> Parameters { get; }
    CommandType CommandType { get; }
    int? CommandTimeout { get; set; }
}

/// <summary>
/// Stored procedure command builder
/// </summary>
public interface IStoredProcedureCommand : ISqlCommand
{
    IStoredProcedureCommand Parameter(string name, object? value);
    IStoredProcedureCommand Parameter(string name, OracleDbType dbType, object? value);
    IStoredProcedureCommand InputParameter(string name, object? value);
    IStoredProcedureCommand OutputParameter(string name, OracleDbType dbType);
    IStoredProcedureCommand InputOutputParameter(string name, OracleDbType dbType, object? value);
    IStoredProcedureCommand ReturnParameter(OracleDbType dbType);
}

/// <summary>
/// SQL command implementation
/// </summary>
public class SqlCommand : ISqlCommand
{
    public string CommandText { get; set; } = "";
    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    public CommandType CommandType { get; set; } = CommandType.Text;
    public int? CommandTimeout { get; set; }
    
    public SqlCommand() { }
    
    public SqlCommand(string commandText, object? parameters = null)
    {
        CommandText = commandText;
        if (parameters != null)
        {
            Parameters = ConvertToParameterDictionary(parameters);
        }
    }
    
    private static Dictionary<string, object?> ConvertToParameterDictionary(object parameters)
    {
        var result = new Dictionary<string, object?>();
        var properties = parameters.GetType().GetProperties();
        
        foreach (var property in properties)
        {
            result[property.Name] = property.GetValue(parameters);
        }
        
        return result;
    }
}

/// <summary>
/// Sort direction enumeration
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Stored procedure result
/// </summary>
public class StoredProcedureResult
{
    public IDictionary<string, object?> OutputParameters { get; set; } = new Dictionary<string, object?>();
    public object? ReturnValue { get; set; }
    public int RecordsAffected { get; set; }
    public IList<dynamic> ResultSets { get; set; } = new List<dynamic>();
}

/// <summary>
/// Strongly-typed stored procedure result
/// </summary>
public class StoredProcedureResult<T> : StoredProcedureResult
{
    public IList<T> Results { get; set; } = new List<T>();
}

/// <summary>
/// Table metadata information
/// </summary>
public class TableMetadata
{
    public string TableName { get; set; } = "";
    public string SchemaName { get; set; } = "";
    public List<ColumnMetadata> Columns { get; set; } = new();
    public List<IndexMetadata> Indexes { get; set; } = new();
    public List<ForeignKeyMetadata> ForeignKeys { get; set; } = new();
    public PrimaryKeyMetadata? PrimaryKey { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModified { get; set; }
    public long RowCount { get; set; }
    public long SizeInBytes { get; set; }
}

/// <summary>
/// Column metadata information
/// </summary>
public class ColumnMetadata
{
    public string ColumnName { get; set; } = "";
    public OracleDbType DataType { get; set; }
    public string DataTypeName { get; set; } = "";
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public bool IsNullable { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsForeignKey { get; set; }
    public object? DefaultValue { get; set; }
    public string? Comments { get; set; }
}

/// <summary>
/// Index metadata information
/// </summary>
public class IndexMetadata
{
    public string IndexName { get; set; } = "";
    public bool IsUnique { get; set; }
    public bool IsPrimary { get; set; }
    public List<string> Columns { get; set; } = new();
    public string IndexType { get; set; } = "";
}

/// <summary>
/// Foreign key metadata information
/// </summary>
public class ForeignKeyMetadata
{
    public string ConstraintName { get; set; } = "";
    public string ColumnName { get; set; } = "";
    public string ReferencedTableName { get; set; } = "";
    public string ReferencedColumnName { get; set; } = "";
    public string OnDeleteAction { get; set; } = "";
    public string OnUpdateAction { get; set; } = "";
}

/// <summary>
/// Primary key metadata information
/// </summary>
public class PrimaryKeyMetadata
{
    public string ConstraintName { get; set; } = "";
    public List<string> Columns { get; set; } = new();
}

/// <summary>
/// Database schema information
/// </summary>
public class SchemaInfo
{
    public string SchemaName { get; set; } = "";
    public List<string> Tables { get; set; } = new();
    public List<string> Views { get; set; } = new();
    public List<string> StoredProcedures { get; set; } = new();
    public List<string> Functions { get; set; } = new();
    public List<string> Sequences { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = new();
}
