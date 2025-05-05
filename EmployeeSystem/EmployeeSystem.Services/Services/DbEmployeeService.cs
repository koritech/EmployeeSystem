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

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync(string? nameFilter, int page, int pageSize)
        {
            var list = await _repo.GetAllAsync(nameFilter, page, pageSize);
            return list.Select(_mapper.ToDto);
        }

        public async Task<EmployeeDto?> GetByNumberAsync(int employeeNumber)
        {
            var entity = await _repo.GetByIdAsync(employeeNumber);
            return entity is null ? null : _mapper.ToDto(entity);
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            var employee = await _repo.GetByIdAsync(dto.EmployeeNumber);
            if (employee is null) return;

            _mapper.UpdateEntity(employee, dto);
            await _repo.UpdateAsync(employee);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int employeeNumber)
        {
            var employee = await _repo.GetByIdAsync(employeeNumber);
            if (employee is null) return;

            await _repo.DeleteAsync(employee);
            await _repo.SaveChangesAsync();
        }
    }
}
