using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FarmsManager.HostBuilder;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next(cancellationToken);

        var context = ValidationContext<TRequest>.CreateWithOptions(request, e => e.IncludeAllRuleSets());
        var validationResult = new List<ValidationResult>();
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            validationResult.Add(result);
        }

        var failures = validationResult.SelectMany(x => x.Errors).Where(x => x != null).ToList();
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}