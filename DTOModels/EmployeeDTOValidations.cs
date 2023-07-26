namespace code_first.DTOModels
{
    using FluentValidation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class EmployeeDTOValidations: AbstractValidator<ImportEmployeeDTO>
    {
        readonly Regex regOnlyLetters = new Regex("^[a-zA-Z ]+$");
        public EmployeeDTOValidations()
        {
            RuleFor(x => x.EmployeeId).Cascade(CascadeMode.Stop)
                .NotEqual(0).WithMessage("Employee Id cannot be empty or 0. ")
                .GreaterThanOrEqualTo(0).WithMessage("Employee Id is not valid. ");

            RuleFor(x => x.DepartmentId).Cascade(CascadeMode.Stop)
                .NotEqual(0).WithMessage("Department Id cannot be empty or 0. ")
                .GreaterThanOrEqualTo(0).WithMessage("Department Id is not valid. ");

            RuleFor(x => x.EmployeeName).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Employee Name cannot be empty. ")
                .Length(1, 50).WithMessage("Employee Name should not be more than 50 characters. ")
                .Matches(regOnlyLetters).WithMessage("Employee Name must contain only alphabets. ");

            //RuleFor(x => x.DepartmentName).Cascade(CascadeMode.Stop)
            //     .NotNull().WithMessage("Department Name cannot be empty. ")
            //     .Length(1, 50).WithMessage("Department Name should not be more than 50 characters. ")
            //     .Matches(regOnlyLetters).WithMessage("Department Name must contain only alphabets. ");
        }
    }
}
