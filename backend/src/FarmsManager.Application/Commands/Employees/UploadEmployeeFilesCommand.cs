using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Employees;

public record UploadEmployeeFilesCommand(Guid Id, IFormFileCollection Files) : IRequest<EmptyBaseResponse>;

public class UploadEmployeeFilesCommandValidator : AbstractValidator<UploadEmployeeFilesCommand>
{
    public UploadEmployeeFilesCommandValidator()
    {
        RuleFor(t => t.Files).NotEmpty();
        RuleFor(t => t.Id).NotEmpty();
    }
}

public class UpdateEmployeeFilesCommandHandler : IRequestHandler<UploadEmployeeFilesCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;

    public UpdateEmployeeFilesCommandHandler(IS3Service s3Service, IUserDataResolver userDataResolver, IEmployeeRepository employeeRepository)
    {
        _s3Service = s3Service;
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UploadEmployeeFilesCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var employee =
            await _employeeRepository.GetAsync(new GetEmployeeByIdWithFilesSpec(request.Id), cancellationToken);

        foreach (var file in request.Files)
        {
            var fileName = Path.GetFileName(file.FileName);
            var filePath = employee.Id + "/" + fileName;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.Employees, filePath);
            employee.AddFile(EmployeeFileEntity.CreateNew(employee.Id, fileName, key, file.ContentType, userId));
        }

        await _employeeRepository.UpdateAsync(employee, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}