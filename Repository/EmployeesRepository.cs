using AutoMapper;
using ClosedXML.Excel;
using code_first.DTOModels;
using code_first.IRepository;
using code_first.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace code_first.Repository
{
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly ProjectDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public EmployeesRepository(ProjectDbContext dbContext, IConfiguration configuration,IMapper mapper)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<DataTable> DownloadEmployee()
        {
            var response = await _dbContext.Employees.Include(emp => emp.Departments).ToListAsync();
            if (response == null)
            {
                return null;
            }
            DataTable dataTableExcel = new DataTable("EmployeeList");
            dataTableExcel.Columns.AddRange(new DataColumn[4] {
                                        new DataColumn("EmployeeId"),
                                        new DataColumn("EmployeeName"),
                                        new DataColumn("DepartmentId"),
                                        new DataColumn("DepartmentName")                                      
                                         });
            //List<ImportEmployeeDTO> ImportEmployeeDTOs = new List<ImportEmployeeDTO>();
            foreach(var employee in response)
            {
                DataRow dataRow = dataTableExcel.NewRow();
                dataRow["EmployeeId"] = employee.EmployeeId;
                dataRow["EmployeeName"] = employee.EmployeeName;
                dataRow["DepartmentId"] = employee.DepartmentId;
                dataRow["DepartmentName"] = employee.Departments.DepartmentName;      
                dataTableExcel.Rows.Add(dataRow);
            }
            return await Task.FromResult(dataTableExcel);
        }

        public async Task<List<Employee>> GetEmployee()
        {
            var response = await _dbContext.Employees.Include(emp => emp.Departments).ToListAsync();
            return response;
        }
        public async Task<DataTable> ExportSeleteddData(EmployeeDTO[] employeeDTO)
        {
            DataTable dataTableExcel = new DataTable("EmployeeList");
            dataTableExcel.Columns.AddRange(new DataColumn[4] {
                                        new DataColumn("EmployeeId"),
                                        new DataColumn("EmployeeName"),
                                        new DataColumn("DepartmentId"),
                                        new DataColumn("DepartmentName")
                                         });
            foreach (var employee in employeeDTO)
            {
                DataRow dataRow = dataTableExcel.NewRow();
                dataRow["EmployeeId"] = employee.EmployeeId;
                dataRow["EmployeeName"] = employee.EmployeeName;
                dataRow["DepartmentId"] = employee.DepartmentId;
                dataRow["DepartmentName"] = employee.Departments.DepartmentName;
                //count++;
                dataTableExcel.Rows.Add(dataRow);
            }
            return await Task.FromResult(dataTableExcel);
        }

        public async Task<List<importEmployeeMessage>> ImportEmployees(IFormFile file)
        {
            var allmessages = new List<importEmployeeMessage>();
            if (file == null)
            {
                var error = new importEmployeeMessage()
                {
                    Message = "No file uploaded."
                };
                allmessages.Add(error);
                return allmessages;
            }
            using var stream = new MemoryStream();
            file.CopyTo(stream);
            XLWorkbook wbook = null;
            try
            {
                wbook = new XLWorkbook(stream);
            }
            catch
            {
                var error = new importEmployeeMessage()
                {
                    Message = "The uploaded file cannot be read. Please upload an excel file."
                };
                allmessages.Add(error);
                return allmessages;
            }

            var ws1 = wbook.Worksheets.FirstOrDefault();
            if (!ws1.IsEmpty())
            {
                var firstRow = ws1.FirstRowUsed().RowNumber();
                List<string> fileColumns = new List<string>();
                List<string> templateColumns = new List<string>();
                var properties = typeof(ImportEmployeeDTO).GetProperties();
                var count = 0; 
                foreach (IXLCell cell in ws1.FirstRowUsed().CellsUsed())
                {
                    fileColumns.Add(cell.Value.ToString().Replace(" ", ""));
                }
                foreach (var prop in properties)
                {
                    templateColumns.Add(prop.Name.ToString());
                }
                if (templateColumns.Count == fileColumns.Count)
                {
                    for (var i = 0; i < templateColumns.Count; i++)
                    {
                        if (fileColumns[i] == templateColumns[i])
                            count++;
                    }
                }
                if (count != templateColumns.Count)
                {
                    var error = new importEmployeeMessage()
                    {
                        Message = "The uploaded file does not follow the template. Please download the template and add users' data."
                    };
                    allmessages.Add(error);
                    return allmessages;
                }

                //reading individual rows
                foreach (var row in ws1.RowsUsed().Skip(1))
                {
                    var obj = new ImportEmployeeDTO();
                    StringBuilder message = new StringBuilder("");
                    string catchprop = null;

                    //mapping cell values to corresponding properties of DTO
                    var colIndex = ws1.FirstColumnUsed().ColumnNumber();
                    foreach (var prop in properties)
                    {
                        var val = row.Cell(colIndex).Value;
                        var type = prop.PropertyType;
                        try
                        {
                            if (val.ToString() == "")
                            {
                                if (type == typeof(int))
                                    val = 0;
                                else
                                    val = null;
                            }
                            prop.SetValue(obj, Convert.ChangeType(val, type));
                        }
                        catch
                        {
                            if (type == typeof(int))
                                message.Append($"{ws1.Cell(firstRow, colIndex).Value} must have numbers only. ");
                            else
                                message.Append($"{ws1.Cell(firstRow, colIndex).Value} is not in correct format. ");
                            val = null;
                            catchprop = ws1.Cell(firstRow, colIndex).Value.ToString();
                            colIndex++;
                            continue;
                        }
                        colIndex++;
                    }

                    //validations for data
                    Employee employee = new Employee();
                    //Employee employee = new Employee();
                    var validator = new EmployeeDTOValidations();
                    var validationResult = validator.Validate(obj);
                    if (obj != null)
                    {
                        employee.EmployeeId = obj.EmployeeId;
                        employee.EmployeeName = obj.EmployeeName;
                        employee.DepartmentId = obj.DepartmentId;
                    }
                    else
                    {
                        foreach (var failure in validationResult.Errors)
                        {
                            if (catchprop != null)
                            {
                                failure.ErrorMessage = failure.ErrorMessage.Contains(catchprop) ? "" : failure.ErrorMessage;
                            }
                            message.Append(failure.ErrorMessage);
                        }
                    }

                    //add or update
                    var ImportUserMessage = new importEmployeeMessage()
                    {
                        EmployeeName = obj.EmployeeName
                    };
                    if (message.ToString() != "")
                    {
                        ImportUserMessage.Message = message.ToString();
                        ImportUserMessage.Status = "Failure";
                        allmessages.Add(ImportUserMessage);
                        continue;
                    }
                    else
                    {
                        //user.ModifiedDate = DateTime.Now;
                        if (_dbContext.Employees.Select(u => u.EmployeeName).Contains(obj.EmployeeName))
                        {
                            var a = _dbContext.Employees.First(x => x.EmployeeName == obj.EmployeeName);
                            _dbContext.Entry(a).State = EntityState.Detached;
                            employee.EmployeeId = a.EmployeeId;
                            _dbContext.Employees.Update(employee);
                            message.Append("User updated.");
                        }
                        else
                        {
                            _dbContext.Employees.Add(employee);
                            message.Append("User added.");
                        }
                        await _dbContext.SaveChangesAsync();
                        ImportUserMessage.Message = message.ToString();
                        ImportUserMessage.Status = "Success";
                    }
                    allmessages.Add(ImportUserMessage);
                }
                if (ws1.RowsUsed().Skip(1).Count() == 0)
                {
                    var error = new importEmployeeMessage()
                    {
                        Message = "The uploaded file has no data to import."
                    };
                    allmessages.Add(error);
                }
            }
            else
            {
                var error = new importEmployeeMessage()
                {
                    Message = "The uploaded file is empty."
                };
                allmessages.Add(error);
            }
            return allmessages;
        }

    }
}
