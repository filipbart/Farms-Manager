---
# API Response Handling Pattern

Standardized API response handling with toast notifications:

```tsx
// Standard pattern - no return value needed
await handleApiResponse(
  () => Service.createItem(data),
  () => {
    toast.success("Success message");
    onSave();
    onClose();
  },
  undefined,
  "Error message" // Always required
);

// When you need the response data
const result = await handleApiResponseWithResult(
  () => Service.getItem(id),
  "Error message" // Always required
);

if (result) {
  // Use result.responseData
}
```

- **handleApiResponse** - For operations where you don't need return data
- **handleApiResponseWithResult** - Returns response data, should be used more often
- **Toast Notifications** - Automatic success/error messages
- **Fallback Required** - fallbackErrorMessage is always required
- **Error Handling** - Shows domainException.errors or fallback message

```tsx
// Response structure
interface BaseResponse<T> {
  success: boolean;
  responseData?: T;
  errors?: Dictionary<string, string>;
  domainException?: { errorDescription: string };
}
```
