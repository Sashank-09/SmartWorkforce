using SmartWorkforce.Domain.Enums;

namespace SmartWorkforce.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
    public DateTime? DueDate { get; set; }
    public Guid AssignedToId { get; set; }
    public Employee? AssignedTo { get; set; }
    public string AssignedById { get; set; } = string.Empty;
}