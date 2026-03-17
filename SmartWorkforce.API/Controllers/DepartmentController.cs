using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWorkforce.Application.DTOs.Department;
using SmartWorkforce.Application.Interfaces;

namespace SmartWorkforce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department == null)
            return NotFound(new { message = "Department not found" });
        return Ok(department);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateDepartmentDto dto)
    {
        var department = await _departmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = department.Id }, department);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateDepartmentDto dto)
    {
        var department = await _departmentService.UpdateAsync(id, dto);
        if (department == null)
            return NotFound(new { message = "Department not found" });
        return Ok(department);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _departmentService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = "Department not found" });
        return Ok(new { message = "Department deleted successfully" });
    }
}