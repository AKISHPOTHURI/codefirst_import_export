namespace code_first.Controllers
{
    using ClosedXML.Excel;
    using code_first.DTOModels;
    using code_first.IRepository;
    using code_first.IService;
    using code_first.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetEmployees:ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        //private readonly IEmployeesRepository _employeeRepository;
        public IConfiguration _configuration { get; }
        public GetEmployees(IEmployeeService employeeService, IConfiguration configuration, IEmployeesRepository employeeRepository)
        {
            _employeeService = employeeService;
            _configuration = configuration;
            //_employeeRepository = employeeRepository;
        }

        [HttpGet("DownloadEmployees")]
        public async Task<IActionResult> DownloadEmployees()
        {
            var response = await _employeeService.DownloadEmployee();

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(response);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }

        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployee()
        {
            var response = await _employeeService.GetEmployee();
            return Ok(response);
        }


        [HttpPost("ExportSeleteddData")]
        public async Task<IActionResult> ExportSeleteddData([FromBody] EmployeeDTO[] employeeDTO)
        {
            var response = await _employeeService.ExportSeleteddData(employeeDTO);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(response);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }

        [HttpPost("ImportEmployees")]
        public async Task<IActionResult> ImportEmployees(IFormFile file)
        {
            var response = await _employeeService.ImportEmployees(file);
            return Ok(response);
        }
    }
}
