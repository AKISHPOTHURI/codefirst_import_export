using code_first.DTOModels;
using code_first.IRepository;
using code_first.IService;
using code_first.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace code_first.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IConfiguration _configuration;
        public EmployeeService(IEmployeesRepository employeesRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _employeesRepository = employeesRepository;
        }
        public async Task<DataTable> DownloadEmployee()
        {
            try
            {
                var response = await _employeesRepository.DownloadEmployee();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<Employee>> GetEmployee()
        {
            try
            {
                var response = await _employeesRepository.GetEmployee();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<DataTable> ExportSeleteddData(EmployeeDTO[] employeeDTO)
        {
            try
            {
                var response = await _employeesRepository.ExportSeleteddData(employeeDTO);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<importEmployeeMessage>> ImportEmployees(IFormFile file)
        {
            try
            {
                var response = await _employeesRepository.ImportEmployees(file);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
