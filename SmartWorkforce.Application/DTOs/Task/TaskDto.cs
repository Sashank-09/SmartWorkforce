using SmartWorkforce.Domain.Enums;

namespace SmartWorkforce.Application.DTOs.Task;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string AssignedToName { get; set; } = string.Empty;
    public Guid AssignedToId { get; set; }
    public string AssignedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
    public DateTime? DueDate { get; set; }
    public Guid AssignedToId { get; set; }
}

public class UpdateTaskStatusDto
{
    public TaskItemStatus Status { get; set; }
}