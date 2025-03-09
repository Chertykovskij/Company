using Company.Core.DTO;
using Company.Database.Entities;

namespace Company.WebApi.Extensions
{
    public static class DepartmentModelExtensions
    {
        public static Department GetDepartment(this DepartmentModel department)
        {
            var departmentEntity = new Department
            {
                Name = department.Name,
                Divisions = new List<Division>()
            };

            foreach (var division in department.Divisions)
            {
                departmentEntity.Divisions.Add(new Division { Name = division.Name });
            }

            return departmentEntity;
        }
    }
}
