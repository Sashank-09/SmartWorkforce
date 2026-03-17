using SmartWorkforce.Application.DTOs.Department;

namespace SmartWorkforce.Application.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(Guid id);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
    Task<DepartmentDto?> UpdateAsync(Guid id, UpdateDepartmentDto dto);
    Task<bool> DeleteAsync(Guid id);
}