using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using MediatR;

namespace FarmsManager.Application.Queries.Employees.Payslips;

public enum EmployeePayslipsOrderBy
{
    FarmName,
    Cycle,
    EmployeeFullName,
    PayrollPeriod,
    NetPay,
    BaseSalary,
    BonusAmount
}

public record GetEmployeePayslipsQueryFilters : OrderedPaginationParams<EmployeePayslipsOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();

    public string SearchPhrase { get; init; }
    public PayrollPeriod? PayrollPeriod { get; init; }
}

public record GetEmployeePayslipsQuery(GetEmployeePayslipsQueryFilters Filters)
    : IRequest<BaseResponse<GetEmployeePayslipsQueryResponse>>;