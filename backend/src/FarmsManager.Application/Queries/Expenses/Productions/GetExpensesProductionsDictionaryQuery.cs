using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Productions;

public class GetExpenseProductionDictionaryQuery : IRequest<BaseResponse<GetExpenseProductionDictionaryQueryResponse>>;

public class GetExpenseProductionDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<DictModel> Contractors { get; set; } = [];
    public List<DictModel> ExpenseTypes { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class GetExpenseProductionDictionaryQueryHandler : IRequestHandler<GetExpenseProductionDictionaryQuery,
    BaseResponse<GetExpenseProductionDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly IExpenseTypeRepository _expenseTypeRepository;

    public GetExpenseProductionDictionaryQueryHandler(
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IExpenseContractorRepository expenseContractorRepository,
        IExpenseTypeRepository expenseTypeRepository)
    {
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _expenseContractorRepository = expenseContractorRepository;
        _expenseTypeRepository = expenseTypeRepository;
    }

    public async Task<BaseResponse<GetExpenseProductionDictionaryQueryResponse>> Handle(
        GetExpenseProductionDictionaryQuery request, CancellationToken ct)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), ct);
        var contractors =
            await _expenseContractorRepository.ListAsync<DictModel>(new GetAllExpenseContractorsSpec(), ct);
        var expenseTypes = await _expenseTypeRepository.ListAsync<DictModel>(new GetAllExpenseTypesSpec(), ct);
        var cycles = await _cycleRepository.ListAsync<CycleDictModel>(new GetAllCyclesSpec(), ct);

        return BaseResponse.CreateResponse(new GetExpenseProductionDictionaryQueryResponse
        {
            Farms = farms,
            Contractors = contractors,
            ExpenseTypes = expenseTypes,
            Cycles = cycles
        });
    }
}

public class ExpensesProductionDictionaryProfile : Profile
{
    public ExpensesProductionDictionaryProfile()
    {
        CreateMap<ExpenseContractorEntity, DictModel>();
        CreateMap<ExpenseTypeEntity, DictModel>();
    }
}