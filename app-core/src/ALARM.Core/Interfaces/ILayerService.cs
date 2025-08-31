namespace ALARM.Core.Interfaces;

/// <summary>
/// Abstraction for AutoCAD layer management operations
/// </summary>
public interface ILayerService
{
    /// <summary>
    /// Gets all layers in the current drawing
    /// </summary>
    Task<IEnumerable<IAcadLayer>> GetAllLayersAsync();
    
    /// <summary>
    /// Gets a layer by name
    /// </summary>
    Task<IAcadLayer?> GetLayerAsync(string name);
    
    /// <summary>
    /// Creates a new layer
    /// </summary>
    Task<IAcadLayer> CreateLayerAsync(string name, LayerProperties? properties = null);
    
    /// <summary>
    /// Updates an existing layer
    /// </summary>
    Task UpdateLayerAsync(IAcadLayer layer);
    
    /// <summary>
    /// Deletes a layer (if not in use)
    /// </summary>
    Task<bool> DeleteLayerAsync(string name, bool force = false);
    
    /// <summary>
    /// Sets the current active layer
    /// </summary>
    Task SetCurrentLayerAsync(string name);
    
    /// <summary>
    /// Gets the current active layer
    /// </summary>
    Task<IAcadLayer> GetCurrentLayerAsync();
    
    /// <summary>
    /// Checks if a layer exists
    /// </summary>
    Task<bool> LayerExistsAsync(string name);
    
    /// <summary>
    /// Gets entities on a specific layer
    /// </summary>
    Task<IEnumerable<IAcadEntity>> GetEntitiesOnLayerAsync(string layerName);
    
    /// <summary>
    /// Moves entities to a different layer
    /// </summary>
    Task MoveEntitiesToLayerAsync(IEnumerable<ObjectId> entityIds, string targetLayerName);
    
    /// <summary>
    /// Freezes/thaws layers
    /// </summary>
    Task SetLayerFrozenAsync(string layerName, bool frozen);
    
    /// <summary>
    /// Locks/unlocks layers
    /// </summary>
    Task SetLayerLockedAsync(string layerName, bool locked);
    
    /// <summary>
    /// Shows/hides layers
    /// </summary>
    Task SetLayerVisibleAsync(string layerName, bool visible);
    
    /// <summary>
    /// Gets layer usage statistics
    /// </summary>
    Task<LayerStats> GetLayerStatsAsync(string layerName);
    
    /// <summary>
    /// Purges unused layers
    /// </summary>
    Task<int> PurgeUnusedLayersAsync();
}

/// <summary>
/// Properties for creating or updating layers
/// </summary>
public class LayerProperties
{
    /// <summary>
    /// Layer color (AutoCAD color index or true color)
    /// </summary>
    public string Color { get; set; } = "7"; // White by default
    
    /// <summary>
    /// Line type name
    /// </summary>
    public string LineType { get; set; } = "Continuous";
    
    /// <summary>
    /// Line weight
    /// </summary>
    public LineWeight LineWeight { get; set; } = LineWeight.Default;
    
    /// <summary>
    /// Transparency (0-255, 0 = opaque)
    /// </summary>
    public byte Transparency { get; set; } = 0;
    
    /// <summary>
    /// Whether layer is plottable
    /// </summary>
    public bool IsPlottable { get; set; } = true;
    
    /// <summary>
    /// Layer description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Custom properties
    /// </summary>
    public Dictionary<string, object> CustomProperties { get; set; } = new();
}

/// <summary>
/// Line weight enumeration
/// </summary>
public enum LineWeight
{
    Default = -3,
    ByLayer = -2,
    ByBlock = -1,
    Weight000 = 0,
    Weight005 = 5,
    Weight009 = 9,
    Weight013 = 13,
    Weight015 = 15,
    Weight018 = 18,
    Weight020 = 20,
    Weight025 = 25,
    Weight030 = 30,
    Weight035 = 35,
    Weight040 = 40,
    Weight050 = 50,
    Weight053 = 53,
    Weight060 = 60,
    Weight070 = 70,
    Weight080 = 80,
    Weight090 = 90,
    Weight100 = 100,
    Weight106 = 106,
    Weight120 = 120,
    Weight140 = 140,
    Weight158 = 158,
    Weight200 = 200,
    Weight211 = 211
}

/// <summary>
/// Layer statistics
/// </summary>
public record LayerStats(
    string LayerName,
    int EntityCount,
    long TotalSize,
    DateTime LastModified,
    bool IsInUse,
    Dictionary<string, int> EntityTypeCounts
);

/// <summary>
/// Layer state for saving/restoring layer configurations
/// </summary>
public class LayerState
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public Dictionary<string, LayerStateInfo> LayerStates { get; set; } = new();
}

/// <summary>
/// Individual layer state information
/// </summary>
public record LayerStateInfo(
    bool IsVisible,
    bool IsFrozen,
    bool IsLocked,
    string Color,
    string LineType,
    LineWeight LineWeight,
    byte Transparency,
    bool IsPlottable
);

/// <summary>
/// Layer management events
/// </summary>
public interface ILayerEvents
{
    /// <summary>
    /// Raised when a layer is created
    /// </summary>
    event EventHandler<LayerEventArgs>? LayerCreated;
    
    /// <summary>
    /// Raised when a layer is deleted
    /// </summary>
    event EventHandler<LayerEventArgs>? LayerDeleted;
    
    /// <summary>
    /// Raised when a layer is modified
    /// </summary>
    event EventHandler<LayerEventArgs>? LayerModified;
    
    /// <summary>
    /// Raised when the current layer changes
    /// </summary>
    event EventHandler<LayerEventArgs>? CurrentLayerChanged;
}

/// <summary>
/// Layer event arguments
/// </summary>
public class LayerEventArgs : EventArgs
{
    public string LayerName { get; }
    public IAcadLayer? Layer { get; }
    public LayerOperation Operation { get; }
    public DateTime Timestamp { get; }
    
    public LayerEventArgs(string layerName, IAcadLayer? layer, LayerOperation operation)
    {
        LayerName = layerName;
        Layer = layer;
        Operation = operation;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Layer operation types
/// </summary>
public enum LayerOperation
{
    Created,
    Deleted,
    Modified,
    SetCurrent,
    Frozen,
    Thawed,
    Locked,
    Unlocked,
    Hidden,
    Shown,
    ColorChanged,
    LineTypeChanged,
    LineWeightChanged
}
