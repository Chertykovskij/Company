namespace Company.Database.Entities
{
    public class Department
    {
        /// <summary>
        /// Идентификатор департамента
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Название департамента
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отделы
        /// </summary>
        public ICollection<Division> Divisions { get; set; } = new List<Division>();
    }
}
