using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Employees;

public record DeleteEmployeeFileCommand(Guid EmployeeId, Guid FileId) : IRequest<EmptyBaseResponse>;

public class DeleteEmployeeFileCommandHandler : IRequestHandler<DeleteEmployeeFileCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeFileRepository _employeeFileRepository;

    public DeleteEmployeeFileCommandHandler(IS3Service s3Service, IEmployeeRepository employeeRepository,
        IEmployeeFileRepository employeeFileRepository)
    {
        _s3Service = s3Service;
        _employeeRepository = employeeRepository;
        _employeeFileRepository = employeeFileRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteEmployeeFileCommand request, CancellationToken cancellationToken)
    {
        var employee =
            await _employeeRepository.GetAsync(new GetEmployeeByIdWithFilesSpec(request.EmployeeId), cancellationToken);

        var employeeFile =
            await _employeeFileRepository.GetAsync(new EmployeeFileByIdSpec(request.FileId), cancellationToken);

        if (employeeFile.EmployeeId != employee.Id)
        {
            throw DomainException.FileNotFound();
        }

        await _s3Service.DeleteFileAsync(FileType.Employees, employeeFile.FilePath);
        await _employeeFileRepository.DeleteAsync(employeeFile, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public sealed class EmployeeFileByIdSpec : BaseSpecification<EmployeeFileEntity>,
    ISingleResultSpecification<EmployeeFileEntity>
{
    public EmployeeFileByIdSpec(Guid fileId)
    {
        EnsureExists();
        Query.Where(e => e.Id == fileId);
    }
}

public class DeleteEmployeeFileCommandValidator : AbstractValidator<DeleteEmployeeFileCommand>
{
    public DeleteEmployeeFileCommandValidator()
    {
        RuleFor(t => t.FileId).NotEmpty();
        RuleFor(t => t.EmployeeId).NotEmpty();
    }
}