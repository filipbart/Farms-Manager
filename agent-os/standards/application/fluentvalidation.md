---
# FluentValidation Pattern

Command validation using FluentValidation library:

```csharp
public class AddFarmCommandValidator : AbstractValidator<AddFarmCommand>
{
    public AddFarmCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.ProdNumber)
            .NotEmpty().WithMessage("Numer producenta jest wymagany.")
            .Must(ValidationHelpers.IsValidProducerOrIrzNumber)
            .WithMessage("Numer producenta musi być w formacie liczba-liczba (np. 00011233-123).");
        RuleFor(t => t.Nip).NotEmpty().Must(ValidationHelpers.IsValidNip)
            .WithMessage("Podany numer NIP jest nieprawidłowy.");
        RuleFor(t => t.Address).NotEmpty();
    }
}
```

- **Not All Commands** - Commands don't always require validators (exceptions for simple commands)
- **AbstractValidator<T>** - All validators inherit from FluentValidation base class
- **Built-in + Custom Rules** - Use both built-in rules (NotEmpty, Must) and custom validation helpers
- **ValidationHelpers** - Custom validation logic in helper methods (IsValidNip, IsValidProducerOrIrzNumber)
- **Error Messages** - Provide Polish error messages with WithMessage()
- **Automatic Integration** - MediatR pipeline automatically runs validators before handlers
