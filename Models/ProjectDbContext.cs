namespace code_first.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProjectDbContext:DbContext
    {
        public ProjectDbContext(DbContextOptions options): base(options)
        {

        }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
    }
}
