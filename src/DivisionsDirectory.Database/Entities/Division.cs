namespace Company.Database.Entities
{
    public class Division
    {
        /// <summary>
        /// Идентификатор отдела
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Название отдела
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор вышестоящего департамента
        /// </summary>
        public long DepartmentId { get; set; }

        /// <summary>
        /// Вышестоящий департамент
        /// </summary>
        public Department Department { get; set; } 
    }
}
