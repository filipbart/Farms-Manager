using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Expenses.Advances;

public record ExpenseAdvanceEntryDto
{
    public DateOnly Date { get; init; }
    public string CategoryName { get; init; }
    public string Name { get; init; }
    public decimal Amount { get; init; }
    public string Comment { get; init; }
    public IFormFile File { get; init; }
}

public record AddExpenseAdvanceData
{
    public ExpenseAdvanceCategoryType Type { get; init; }
    public List<ExpenseAdvanceEntryDto> Entries { get; init; } = [];
}

public record AddExpenseAdvanceCommand(Guid EmployeeId, AddExpenseAdvanceData Data) : IRequest<EmptyBaseResponse>;

public class AddExpenseAdvanceCommandHandler : IRequestHandler<AddExpenseAdvanceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IExpenseAdvanceCategoryRepository _categoryRepository;
    private readonly IExpenseAdvanceRepository _advanceRepository;
    private readonly IS3Service _s3Service;

    public AddExpenseAdvanceCommandHandler(
        IUserDataResolver userDataResolver,
        IEmployeeRepository employeeRepository,
        IExpenseAdvanceCategoryRepository categoryRepository,
        IExpenseAdvanceRepository advanceRepository,
        IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
        _categoryRepository = categoryRepository;
        _advanceRepository = advanceRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(AddExpenseAdvanceCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var employee = await _employeeRepository.GetAsync(new EmployeeByIdSpec(request.EmployeeId), ct);

        var newAdvances = new List<ExpenseAdvanceEntity>();

        foreach (var entry in request.Data.Entries)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(
                               new GetAdvanceCategoryByNameAndTypeSpec(entry.CategoryName, request.Data.Type), ct) ??
                           throw new Exception(
                               $"Kategoria '{entry.CategoryName}' o typie '{request.Data.Type}' nie istnieje.");

            string filePath = null;
            if (entry.File is not null)
            {
                var fileId = Guid.NewGuid();
                var extension = Path.GetExtension(entry.File.FileName);
                var path = $"{employee.Id}/{fileId}{extension}";

                using var memoryStream = new MemoryStream();
                await entry.File.CopyToAsync(memoryStream, ct);

                filePath = await _s3Service.UploadFileAsync(memoryStream.ToArray(), FileType.ExpenseAdvance, path);
            }

            var newAdvance = ExpenseAdvanceEntity.CreateNew(
                employee.Id,
                category.Id,
                entry.Date,
                request.Data.Type,
                entry.Name,
                entry.Amount,
                entry.Comment,
                filePath,
                userId
            );
            newAdvances.Add(newAdvance);
        }

        await _advanceRepository.AddRangeAsync(newAdvances, ct);

        return BaseResponse.EmptyResponse;
    }
}

public class AddExpenseAdvanceCommandValidator : AbstractValidator<AddExpenseAdvanceCommand>
{
    public AddExpenseAdvanceCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Data.Type).IsInEnum();
        RuleFor(x => x.Data.Entries).NotEmpty().WithMessage("Należy dodać przynajmniej jeden wpis.");

        RuleForEach(x => x.Data.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.Date).NotEmpty();
            entry.RuleFor(e => e.CategoryName).NotEmpty();
            entry.RuleFor(e => e.Name).NotEmpty();
            entry.RuleFor(e => e.Amount).GreaterThan(0);
        });
    }
}

public sealed class GetAdvanceCategoryByNameAndTypeSpec : BaseSpecification<ExpenseAdvanceCategoryEntity>,
    ISingleResultSpecification<ExpenseAdvanceCategoryEntity>
{
    public GetAdvanceCategoryByNameAndTypeSpec(string name, ExpenseAdvanceCategoryType type)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(c => c.Name == name && c.Type == type);
    }
}