namespace Company.Core.DTO
{
    public class DepartmentModel
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отделы
        /// </summary>
        public ICollection<DivisionModel> Divisions { get; set; } = new List<DivisionModel>();
    }
}
