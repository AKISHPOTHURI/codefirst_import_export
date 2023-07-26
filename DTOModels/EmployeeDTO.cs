namespace code_first.DTOModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int DepartmentId { get; set; }
        public Department Departments { get; set; }

    }

    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
