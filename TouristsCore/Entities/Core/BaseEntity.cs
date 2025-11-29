namespace TouristsCore.Entities;

public class BaseEntity : ISoftDeletable
{
    public int Id { get; set; }
    
    // For Cursor Pagination (Sort by this)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // For Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public void UndoDelete()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}