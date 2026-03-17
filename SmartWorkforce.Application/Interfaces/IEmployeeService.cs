using SmartWorkforce.Application.DTOs.Employee;

namespace SmartWorkforce.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto);
    Task<bool> DeleteAsync(Guid id);
}