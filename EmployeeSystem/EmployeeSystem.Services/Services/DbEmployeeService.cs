using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Mappers;

namespace EmployeeSystem.Services
{
    public class DbEmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IEmployeeMapper _mapper;

        public DbEmployeeService(IEmployeeRepository repo, IEmployeeMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task AddAsync(EmployeeDto dto)
        {
            var employee = _mapper.ToEntity(dto);
            await _repo.AddAsync(employee);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            var employee = await _repo.GetByIdAsync(dto.EmployeeNumber);
            if (employee is null) return;

            _mapper.UpdateEntity(employee, dto);
            await _repo.UpdateAsync(employee);
            await _repo.SaveChangesAsync();
        }
    }
}
