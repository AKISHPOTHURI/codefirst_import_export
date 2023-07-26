using code_first.DTOModels;
using code_first.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace code_first.IService
{
    public interface IEmployeeService
    {
        public Task<DataTable> DownloadEmployee();
        public Task<List<Employee>> GetEmployee();
        
        public Task<DataTable> ExportSeleteddData(EmployeeDTO[] employeeDTO);
        public Task<List<importEmployeeMessage>> ImportEmployees(IFormFile file);

    }
}
