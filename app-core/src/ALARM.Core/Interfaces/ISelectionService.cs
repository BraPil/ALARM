namespace ALARM.Core.Interfaces;

/// <summary>
/// Abstraction for AutoCAD selection operations
/// </summary>
public interface ISelectionService
{
    /// <summary>
    /// Prompts user to select entities
    /// </summary>
    Task<ISelectionSet> GetSelectionAsync(string? prompt = null);
    
    /// <summary>
    /// Selects entities by criteria without user interaction
    /// </summary>
    Task<ISelectionSet> SelectByCriteriaAsync(SelectionCriteria criteria);
    
    /// <summary>
    /// Selects all entities in the drawing
    /// </summary>
    Task<ISelectionSet> SelectAllAsync();
    
    /// <summary>
    /// Selects entities within a window
    /// </summary>
    Task<ISelectionSet> SelectWindowAsync(Point3D corner1, Point3D corner2);
    
    /// <summary>
    /// Selects entities crossing a window
    /// </summary>
    Task<ISelectionSet> SelectCrossingAsync(Point3D corner1, Point3D corner2);
    
    /// <summary>
    /// Clears the current selection
    /// </summary>
    Task ClearSelectionAsync();
    
    /// <summary>
    /// Highlights the selected entities
    /// </summary>
    Task HighlightSelectionAsync(ISelectionSet selectionSet, bool highlight = true);
}

/// <summary>
/// Represents a set of selected entities
/// </summary>
public interface ISelectionSet : IEnumerable<ObjectId>, IDisposable
{
    /// <summary>
    /// Number of selected entities
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// Gets entity at specified index
    /// </summary>
    ObjectId this[int index] { get; }
    
    /// <summary>
    /// Adds an entity to the selection
    /// </summary>
    void Add(ObjectId objectId);
    
    /// <summary>
    /// Removes an entity from the selection
    /// </summary>
    bool Remove(ObjectId objectId);
    
    /// <summary>
    /// Clears all selected entities
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Checks if an entity is selected
    /// </summary>
    bool Contains(ObjectId objectId);
    
    /// <summary>
    /// Gets all object IDs as an array
    /// </summary>
    ObjectId[] ToArray();
    
    /// <summary>
    /// Gets the entities as a list
    /// </summary>
    Task<IList<IAcadEntity>> GetEntitiesAsync(IAcadTransaction transaction);
}

/// <summary>
/// Criteria for entity selection
/// </summary>
public class SelectionCriteria
{
    /// <summary>
    /// Entity types to include
    /// </summary>
    public List<string> EntityTypes { get; set; } = new();
    
    /// <summary>
    /// Layer names to include
    /// </summary>
    public List<string> LayerNames { get; set; } = new();
    
    /// <summary>
    /// Property filters
    /// </summary>
    public Dictionary<string, object> PropertyFilters { get; set; } = new();
    
    /// <summary>
    /// Bounding box to limit selection
    /// </summary>
    public BoundingBox? BoundingBox { get; set; }
    
    /// <summary>
    /// Whether to include locked layers
    /// </summary>
    public bool IncludeLockedLayers { get; set; } = false;
    
    /// <summary>
    /// Whether to include frozen layers
    /// </summary>
    public bool IncludeFrozenLayers { get; set; } = false;
    
    /// <summary>
    /// Whether to include invisible entities
    /// </summary>
    public bool IncludeInvisible { get; set; } = false;
    
    /// <summary>
    /// Maximum number of entities to select (0 = unlimited)
    /// </summary>
    public int MaxCount { get; set; } = 0;
}

/// <summary>
/// Selection mode enumeration
/// </summary>
public enum SelectionMode
{
    /// <summary>
    /// User picks individual entities
    /// </summary>
    Pick,
    
    /// <summary>
    /// Window selection (entities completely inside)
    /// </summary>
    Window,
    
    /// <summary>
    /// Crossing selection (entities crossing or inside)
    /// </summary>
    Crossing,
    
    /// <summary>
    /// Fence selection (entities crossing a line)
    /// </summary>
    Fence,
    
    /// <summary>
    /// Polygon selection
    /// </summary>
    Polygon,
    
    /// <summary>
    /// All entities in drawing
    /// </summary>
    All,
    
    /// <summary>
    /// Last created entity
    /// </summary>
    Last,
    
    /// <summary>
    /// Previous selection set
    /// </summary>
    Previous
}

/// <summary>
/// Selection event arguments
/// </summary>
public class SelectionEventArgs : EventArgs
{
    public ISelectionSet SelectionSet { get; }
    public SelectionMode Mode { get; }
    public DateTime Timestamp { get; }
    
    public SelectionEventArgs(ISelectionSet selectionSet, SelectionMode mode)
    {
        SelectionSet = selectionSet;
        Mode = mode;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Selection service events
/// </summary>
public interface ISelectionEvents
{
    /// <summary>
    /// Raised when selection changes
    /// </summary>
    event EventHandler<SelectionEventArgs>? SelectionChanged;
    
    /// <summary>
    /// Raised when selection is cleared
    /// </summary>
    event EventHandler? SelectionCleared;
    
    /// <summary>
    /// Raised when selection operation starts
    /// </summary>
    event EventHandler<SelectionEventArgs>? SelectionStarted;
    
    /// <summary>
    /// Raised when selection operation completes
    /// </summary>
    event EventHandler<SelectionEventArgs>? SelectionCompleted;
}
