using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Models;

namespace EmployeeSystem.Services.Services
{
    public class EmployeeQueryService : IEmployeeQueryService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IEmployeeMapper _mapper;

        public EmployeeQueryService(IEmployeeRepository repo,IEmployeeMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EmployeePagedResult> GetAllEmployeesPagedAsync(string? nameFilter, int page, int pageSize)
        {
            var (data, totalCount) = await _repo.GetAllWithCountAsync(nameFilter, page, pageSize);

            return new EmployeePagedResult
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data.Select(_mapper.ToDto)
            };
        }


        public async Task<EmployeeDto?> GetByEmployeeNumberAsync(int employeeNumber)
        {
            var entity = await _repo.GetByIdAsync(employeeNumber);
            return entity is null ? null : _mapper.ToDto(entity);
        }

        public async Task DeleteEmployeeAsync(int employeeNumber)
        {
            var employee = await _repo.GetByIdAsync(employeeNumber);
            if (employee is null) return;

            await _repo.DeleteAsync(employee);
            await _repo.SaveChangesAsync();
        }
    }

}
