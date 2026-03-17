using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartWorkforce.Application.DTOs.Employee;
using SmartWorkforce.Application.Interfaces;
using SmartWorkforce.Domain.Entities;
using SmartWorkforce.Infrastructure.Data;

namespace SmartWorkforce.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EmployeeService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => !e.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        return employee == null
            ? null
            : _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId
                && !d.IsDeleted);

        if (department == null)
            throw new Exception("Department not found");

        var employee = _mapper.Map<Employee>(dto);
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        await _context.Entry(employee)
            .Reference(e => e.Department)
            .LoadAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto?> UpdateAsync(
        Guid id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        if (employee == null) return null;

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId
                && !d.IsDeleted);

        if (department == null)
            throw new Exception("Department not found");

        _mapper.Map(dto, employee);
        employee.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        if (employee == null) return false;

        employee.IsDeleted = true;
        employee.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}