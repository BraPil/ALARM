namespace ALARM.Core.Interfaces;

/// <summary>
/// Abstraction for AutoCAD Map 3D integration services
/// </summary>
public interface IAutoCadService
{
    /// <summary>
    /// Gets the current active document
    /// </summary>
    Task<IAcadDocument?> GetActiveDocumentAsync();
    
    /// <summary>
    /// Opens a drawing file
    /// </summary>
    Task<IAcadDocument> OpenDrawingAsync(string filePath);
    
    /// <summary>
    /// Creates a new drawing
    /// </summary>
    Task<IAcadDocument> CreateNewDrawingAsync(string? templatePath = null);
    
    /// <summary>
    /// Gets the application version information
    /// </summary>
    string GetVersion();
    
    /// <summary>
    /// Checks if AutoCAD is running and available
    /// </summary>
    Task<bool> IsAvailableAsync();
}

/// <summary>
/// Represents an AutoCAD document
/// </summary>
public interface IAcadDocument : IDisposable
{
    /// <summary>
    /// Document name
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Full file path
    /// </summary>
    string? FilePath { get; }
    
    /// <summary>
    /// Indicates if document has unsaved changes
    /// </summary>
    bool IsDirty { get; }
    
    /// <summary>
    /// Gets the database service for this document
    /// </summary>
    IAcadDatabase Database { get; }
    
    /// <summary>
    /// Gets the selection service for this document
    /// </summary>
    ISelectionService Selection { get; }
    
    /// <summary>
    /// Gets the layer service for this document
    /// </summary>
    ILayerService Layers { get; }
    
    /// <summary>
    /// Saves the document
    /// </summary>
    Task SaveAsync();
    
    /// <summary>
    /// Saves the document to a specific path
    /// </summary>
    Task SaveAsAsync(string filePath);
    
    /// <summary>
    /// Closes the document
    /// </summary>
    Task CloseAsync(bool saveChanges = true);
    
    /// <summary>
    /// Starts a new transaction
    /// </summary>
    Task<IAcadTransaction> StartTransactionAsync();
}

/// <summary>
/// Represents an AutoCAD database transaction
/// </summary>
public interface IAcadTransaction : IDisposable
{
    /// <summary>
    /// Commits the transaction
    /// </summary>
    Task CommitAsync();
    
    /// <summary>
    /// Aborts the transaction
    /// </summary>
    Task AbortAsync();
    
    /// <summary>
    /// Gets an entity by its object ID
    /// </summary>
    Task<IAcadEntity?> GetEntityAsync(ObjectId objectId);
    
    /// <summary>
    /// Adds an entity to the database
    /// </summary>
    Task<ObjectId> AddEntityAsync(IAcadEntity entity, string? layerName = null);
    
    /// <summary>
    /// Updates an entity in the database
    /// </summary>
    Task UpdateEntityAsync(IAcadEntity entity);
    
    /// <summary>
    /// Deletes an entity from the database
    /// </summary>
    Task DeleteEntityAsync(ObjectId objectId);
}

/// <summary>
/// Abstraction for AutoCAD database operations
/// </summary>
public interface IAcadDatabase
{
    /// <summary>
    /// Gets all layers in the database
    /// </summary>
    Task<IEnumerable<IAcadLayer>> GetLayersAsync();
    
    /// <summary>
    /// Gets all blocks in the database
    /// </summary>
    Task<IEnumerable<IAcadBlock>> GetBlocksAsync();
    
    /// <summary>
    /// Queries entities by criteria
    /// </summary>
    Task<IEnumerable<IAcadEntity>> QueryEntitiesAsync(EntityQuery query);
    
    /// <summary>
    /// Gets database statistics
    /// </summary>
    Task<DatabaseStats> GetStatsAsync();
}

/// <summary>
/// Represents a unique object identifier in AutoCAD
/// </summary>
public readonly struct ObjectId : IEquatable<ObjectId>
{
    private readonly long _value;
    
    public ObjectId(long value) => _value = value;
    
    public bool IsNull => _value == 0;
    public bool IsValid => _value > 0;
    
    public static ObjectId Null => new(0);
    
    public bool Equals(ObjectId other) => _value == other._value;
    public override bool Equals(object? obj) => obj is ObjectId other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => $"ObjectId({_value})";
    
    public static bool operator ==(ObjectId left, ObjectId right) => left.Equals(right);
    public static bool operator !=(ObjectId left, ObjectId right) => !left.Equals(right);
}

/// <summary>
/// Base interface for AutoCAD entities
/// </summary>
public interface IAcadEntity
{
    ObjectId Id { get; }
    string EntityType { get; }
    string LayerName { get; set; }
    bool IsVisible { get; set; }
    Dictionary<string, object> Properties { get; }
}

/// <summary>
/// Represents an AutoCAD layer
/// </summary>
public interface IAcadLayer
{
    string Name { get; set; }
    bool IsVisible { get; set; }
    bool IsLocked { get; set; }
    bool IsFrozen { get; set; }
    string Color { get; set; }
    string LineType { get; set; }
}

/// <summary>
/// Represents an AutoCAD block definition
/// </summary>
public interface IAcadBlock
{
    string Name { get; }
    string Description { get; set; }
    IEnumerable<IAcadEntity> Entities { get; }
}

/// <summary>
/// Query criteria for entity searches
/// </summary>
public class EntityQuery
{
    public string? EntityType { get; set; }
    public string? LayerName { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public BoundingBox? Bounds { get; set; }
}

/// <summary>
/// Represents a 3D bounding box
/// </summary>
public record BoundingBox(Point3D Min, Point3D Max);

/// <summary>
/// Represents a 3D point
/// </summary>
public record Point3D(double X, double Y, double Z);

/// <summary>
/// Database statistics
/// </summary>
public record DatabaseStats(
    int EntityCount,
    int LayerCount,
    int BlockCount,
    long FileSize,
    DateTime LastModified
);
