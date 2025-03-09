using Company.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.Database
{
    public class CompanyDbContext : DbContext
    {
        public CompanyDbContext() { }

        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>(department =>
            {
                department.ToTable("Departments");
                department.HasKey(d => d.Id);
                department.Property(p => p.Id).ValueGeneratedOnAdd().HasColumnName("id");
                department.Property(p => p.Name).HasColumnName("name").IsRequired(true);
            });

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Divisions)
                .WithOne(dv => dv.Department)
                .HasForeignKey(dv => dv.DepartmentId);
        }
    }
}
