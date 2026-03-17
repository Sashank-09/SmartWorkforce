using AutoMapper;
using SmartWorkforce.Application.DTOs.Department;
using SmartWorkforce.Application.DTOs.Employee;
using SmartWorkforce.Application.DTOs.Task;
using SmartWorkforce.Domain.Entities;

namespace SmartWorkforce.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Department mappings
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.EmployeeCount,
                opt => opt.MapFrom(src => src.Employees.Count));
        CreateMap<CreateDepartmentDto, Department>();
        CreateMap<UpdateDepartmentDto, Department>();

        // Employee mappings
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src =>
                    src.Department != null
                        ? src.Department.Name
                        : string.Empty));
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<UpdateEmployeeDto, Employee>();

        // Task mappings
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.AssignedToName,
                opt => opt.MapFrom(src =>
                    src.AssignedTo != null
                        ? $"{src.AssignedTo.FirstName} {src.AssignedTo.LastName}"
                        : string.Empty));
        CreateMap<CreateTaskDto, TaskItem>();
    }
}