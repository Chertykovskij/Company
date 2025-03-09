using Company.WebApi.Abstractions;
using Company.Database;
using Company.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.WebApi.Database
{
    public class DepartmentRepository : IRepository
    {
        private readonly CompanyDbContext _dbContext;

        public DepartmentRepository(CompanyDbContext context)
        {
            _dbContext = context;
        }

        public async Task<long> AddDepartment(Department department)
        {
            _dbContext.Departments.Add(department);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<Department> GetDepartment(long idDepartment)
        {
            return await _dbContext.Departments.FindAsync(idDepartment);
        }

        public async Task<IEnumerable<Department>> GetDepartments()
        {
            return await _dbContext.Departments.ToListAsync();
        }
    }
}
