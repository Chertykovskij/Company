using Company.Database.Entities;

namespace Company.WebApi.Abstractions
{
    public interface IRepository
    {
        /// <summary>
        /// Получить информацию о всех департаментах
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Department>> GetDepartments();

        /// <summary>
        /// Получить информацию о департаменте
        /// </summary>
        /// <param name="idDepartment">Идентификатор департамента</param>
        /// <returns></returns>
        public Task<Department> GetDepartment(long idDepartment);

        /// <summary>
        /// Добавление департамента
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public Task<long> AddDepartment(Department department);
    }
}
