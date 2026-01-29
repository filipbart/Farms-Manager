---
# Error Handling in Wrapper Pattern

Centralized error handling in AxiosWrapper:

```tsx
const handleErrorResponse: <T>(error: any) => BaseResponse<T> = (error: any) => {
  let domainException: DomainException | undefined = undefined;
  if (error?.response?.data?.errorName) {
    domainException = error.response.data;
  }

  return {
    success: false,
    domainException: domainException,
    statusCode: error?.response?.status || 500,
    responseData: {} as any,
    errors: error?.response?.data?.errors || [],
  };
};
```

- **DomainException Handling** - Captures backend domain exceptions with errorName
- **Standard Error Structure** - All errors follow BaseResponse format
- **Multiple Error Types** - errors dictionary can contain validation errors, business errors, etc.
- **Status Code Tracking** - Preserves HTTP status codes for error handling
- **Consistent Interface** - Always returns BaseResponse<T> regardless of success/failure

```tsx
// Backend DomainException structure
{
  errorName: "RecordNotFound",
  errorDescription: "Podmiot gospodarczy not found",
  statusCode: 404
}

// Frontend receives
{
  success: false,
  domainException: { errorName, errorDescription },
  statusCode: 404,
  errors: []
}
```
