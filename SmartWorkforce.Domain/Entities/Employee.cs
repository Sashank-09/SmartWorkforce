namespace SmartWorkforce.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string? UserId { get; set; }
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}