using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartWorkforce.Application.DTOs.Task;
using SmartWorkforce.Application.Interfaces;
using SmartWorkforce.Domain.Entities;
using SmartWorkforce.Infrastructure.Data;

namespace SmartWorkforce.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public TaskService(
        AppDbContext context,
        IMapper mapper,
        INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<TaskDto>> GetAllAsync()
    {
        var tasks = await _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }

    public async Task<IEnumerable<TaskDto>> GetMyTasksAsync(string userId)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e =>
                e.UserId == userId && !e.IsDeleted);

        if (employee == null)
            return Enumerable.Empty<TaskDto>();

        var tasks = await _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.AssignedToId == employee.Id && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _context.Tasks
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        return task == null ? null : _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateAsync(
        CreateTaskDto dto, string assignedById)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e =>
                e.Id == dto.AssignedToId && !e.IsDeleted);

        if (employee == null)
            throw new Exception("Employee not found");

        var task = _mapper.Map<TaskItem>(dto);
        task.AssignedById = assignedById;

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        await _context.Entry(task)
            .Reference(t => t.AssignedTo)
            .LoadAsync();

        // Send real-time notification to employee
        if (!string.IsNullOrEmpty(employee.UserId))
        {
            var assignedByUser = await _context.Users
                .FindAsync(assignedById);
            var assignedByName = assignedByUser != null
                ? assignedById
                : "Manager";

            await _notificationService
                .SendTaskAssignedNotificationAsync(
                    employee.UserId,
                    task.Title,
                    assignedByName);
        }

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto?> UpdateStatusAsync(
        Guid id, UpdateTaskStatusDto dto, string userId)
    {
        var task = await _context.Tasks
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null) return null;

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e =>
                e.UserId == userId && !e.IsDeleted);

        var userRole = _context.Users
            .Where(u => u.Id == userId)
            .Select(u =>
                ((SmartWorkforce.Infrastructure.Identity
                    .ApplicationUser)u).Role)
            .FirstOrDefault();

        bool isAdminOrManager =
            userRole == "Admin" || userRole == "Manager";
        bool isOwner = employee != null &&
            task.AssignedToId == employee.Id;

        if (!isAdminOrManager && !isOwner)
            throw new UnauthorizedAccessException(
                "You can only update tasks assigned to you");

        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Send notification to the manager who assigned the task
        if (!string.IsNullOrEmpty(task.AssignedById))
        {
            var employeeName = task.AssignedTo != null
                ? $"{task.AssignedTo.FirstName} {task.AssignedTo.LastName}"
                : "Employee";

            await _notificationService
                .SendTaskStatusUpdatedNotificationAsync(
                    task.AssignedById,
                    task.Title,
                    dto.Status.ToString(),
                    employeeName);
        }

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null) return false;

        task.IsDeleted = true;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}