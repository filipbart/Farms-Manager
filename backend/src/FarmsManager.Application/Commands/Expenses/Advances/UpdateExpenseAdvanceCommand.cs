using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Services;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Expenses.Advances;

public record UpdateExpenseAdvanceData
{
    public DateOnly Date { get; init; }
    public ExpenseAdvanceCategoryType Type { get; init; }
    public string Name { get; init; }
    public decimal Amount { get; init; }
    public string CategoryName { get; init; }
    public string Comment { get; init; }
    public IFormFile File { get; init; }
}

public record UpdateExpenseAdvanceCommand(Guid Id, UpdateExpenseAdvanceData Data) : IRequest<EmptyBaseResponse>;

public class UpdateExpenseAdvanceCommandHandler : IRequestHandler<UpdateExpenseAdvanceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseAdvanceRepository _advanceRepository;
    private readonly IExpenseAdvanceCategoryRepository _categoryRepository;
    private readonly IS3Service _s3Service;
    private readonly IExpenseAdvancePermissionService _permissionService;

    public UpdateExpenseAdvanceCommandHandler(
        IUserDataResolver userDataResolver,
        IExpenseAdvanceRepository advanceRepository,
        IExpenseAdvanceCategoryRepository categoryRepository,
        IS3Service s3Service,
        IExpenseAdvancePermissionService permissionService)
    {
        _userDataResolver = userDataResolver;
        _advanceRepository = advanceRepository;
        _categoryRepository = categoryRepository;
        _s3Service = s3Service;
        _permissionService = permissionService;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateExpenseAdvanceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity = await _advanceRepository.GetAsync(new GetExpenseAdvanceByIdSpec(request.Id), cancellationToken);

        // Sprawdź uprawnienia do edycji ewidencji tego pracownika
        var hasPermission = await _permissionService.HasPermissionAsync(
            userId, 
            entity.EmployeeId, 
            ExpenseAdvancePermissionType.Edit,
            cancellationToken);

        if (!hasPermission)
            throw DomainException.Forbidden();

        var category = await _categoryRepository.FirstOrDefaultAsync(
                           new GetAdvanceCategoryByNameAndTypeSpec(request.Data.CategoryName, request.Data.Type),
                           cancellationToken)
                       ?? throw new Exception(
                           $"Kategoria '{request.Data.CategoryName}' o typie '{request.Data.Type}' nie istnieje.");

        var newFilePath = entity.FilePath;


        if (request.Data.File is not null)
        {
            if (entity.FilePath.IsNotEmpty())
            {
                await _s3Service.DeleteFileAsync(FileType.ExpenseAdvance, entity.FilePath);
            }

            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(request.Data.File.FileName);
            var path = $"{entity.EmployeeId}/{fileId}{extension}";

            using var memoryStream = new MemoryStream();
            await request.Data.File.CopyToAsync(memoryStream, cancellationToken);

            newFilePath = await _s3Service.UploadFileAsync(memoryStream.ToArray(), FileType.ExpenseAdvance, path);
        }

        entity.Update(
            request.Data.Date,
            request.Data.Type,
            request.Data.Name,
            request.Data.Amount,
            request.Data.Comment,
            newFilePath
        );
        entity.SetExpenseAdvanceCategory(category.Id);
        entity.SetModified(userId);

        await _advanceRepository.UpdateAsync(entity, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class UpdateExpenseAdvanceCommandValidator : AbstractValidator<UpdateExpenseAdvanceCommand>
{
    public UpdateExpenseAdvanceCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.Date).NotEmpty();
        RuleFor(x => x.Data.Type).IsInEnum();
        RuleFor(x => x.Data.Name).NotEmpty();
        RuleFor(x => x.Data.CategoryName).NotEmpty();
        RuleFor(x => x.Data.Amount).GreaterThan(0);
    }
}