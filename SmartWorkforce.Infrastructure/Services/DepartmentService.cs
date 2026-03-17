using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartWorkforce.Application.DTOs.Department;
using SmartWorkforce.Application.Interfaces;
using SmartWorkforce.Domain.Entities;
using SmartWorkforce.Infrastructure.Data;

namespace SmartWorkforce.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DepartmentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.Employees)
            .Where(d => !d.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto?> GetByIdAsync(Guid id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        return department == null
            ? null
            : _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        var department = _mapper.Map<Department>(dto);
        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();
        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto?> UpdateAsync(
        Guid id, UpdateDepartmentDto dto)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (department == null) return null;

        _mapper.Map(dto, department);
        department.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (department == null) return false;

        department.IsDeleted = true;
        department.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}