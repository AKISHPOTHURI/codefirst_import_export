namespace code_first.Mapper
{
    using AutoMapper;
    using code_first.DTOModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<System.Data.DataRow, EmployeeDTO>()
                    .ForMember(m => m.EmployeeId, s => s.MapFrom(s => (int)s.ItemArray[0]))
                    .ForMember(m => m.EmployeeName, s => s.MapFrom(s => s.ItemArray[1].ToString()))
                    .ForMember(m => m.DepartmentId, s => s.MapFrom(s => (int)s.ItemArray[2]))
                    .ReverseMap();

            CreateMap<System.Data.DataRow, Department>()
                    .ForMember(m => m.DepartmentId, s => s.MapFrom(s => (int)s.ItemArray[0]))
                    .ForMember(m => m.DepartmentName, s => s.MapFrom(s => s.ItemArray[1].ToString()))
                    .ReverseMap();
        }
        
    }
}
