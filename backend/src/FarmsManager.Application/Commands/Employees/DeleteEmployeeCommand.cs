using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Commands.Employees;

public record DeleteEmployeeCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteEmployeeCommandHandler(IUserDataResolver userDataResolver,
        IEmployeeRepository employeeRepository, IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _employeeRepository = employeeRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var entity =
            await _employeeRepository.GetAsync(new GetEmployeeByIdWithFilesSpec(request.Id),
                cancellationToken);

        foreach (var file in entity.Files)
        {
            if (file.FilePath.IsNotEmpty())
            {
                await _s3Service.DeleteFileAsync(FileType.Employees, file.FilePath);
            }
        }

        entity.Delete(userId);
        await _employeeRepository.UpdateAsync(entity, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}