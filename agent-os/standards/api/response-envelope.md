---
# API Response Envelope

All API responses use BaseResponse envelope:

```csharp
BaseResponse<T> { Success, Errors, ResponseData, ResponseTimeUtc }
EmptyBaseResponse { Success, Errors, ResponseTimeUtc }
```

- `Success` is computed: true when `Errors.Count == 0`
- `ResponseTimeUtc` is for debugging/logging only, not used by frontend
- Never return raw data without the envelope
- Use `EmptyBaseResponse` for actions with no return data

```csharp
// Success response
return Ok(await mediator.Send(query));

// Error response (handled by middleware)
// Errors are added to response.Errors dictionary
```
