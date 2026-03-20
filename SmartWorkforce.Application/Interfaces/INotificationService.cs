namespace SmartWorkforce.Application.Interfaces;

public interface INotificationService
{
    Task SendTaskAssignedNotificationAsync(
        string employeeUserId,
        string taskTitle,
        string assignedByName);

    Task SendTaskStatusUpdatedNotificationAsync(
        string managerUserId,
        string taskTitle,
        string newStatus,
        string employeeName);
}