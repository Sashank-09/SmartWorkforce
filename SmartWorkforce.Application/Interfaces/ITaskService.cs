using SmartWorkforce.Application.DTOs.Task;

namespace SmartWorkforce.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync();
    Task<IEnumerable<TaskDto>> GetMyTasksAsync(string userId);
    Task<TaskDto?> GetByIdAsync(Guid id);
    Task<TaskDto> CreateAsync(CreateTaskDto dto, string assignedById);
    Task<TaskDto?> UpdateStatusAsync(
        Guid id, UpdateTaskStatusDto dto, string userId);
    Task<bool> DeleteAsync(Guid id);
}