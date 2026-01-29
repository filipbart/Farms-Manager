---
# AxiosWrapper Pattern

Centralized HTTP client with BaseResponse handling:

```tsx
export default class AxiosWrapper {
  public static get<T>(url: string, params?: any): Promise<BaseResponse<T>>
  public static post<T>(url: string, data?: any): Promise<BaseResponse<T>>
  public static patch<T>(url: string, data?: any): Promise<BaseResponse<T>>
  public static put<T>(url: string, data?: any): Promise<BaseResponse<T>>
  public static delete<T>(url: string, params?: any): Promise<BaseResponse<T>>

  // Token management
  public static setAuthToken(token: string): void
  public static getAuthTokenFromHeader(): string
}
```

- **BaseResponse Wrapping** - All responses wrapped in BaseResponse<T> with success/errors
- **Error Handling** - Centralized error handling with domainException support
- **Token Management** - Automatic JWT token handling in headers
- **Consistent Interface** - All HTTP methods follow same pattern
- **Delete Method** - Uses `data` in options for backend consistency
- **Request Cancellation** - Supports axios.isCancel for request cancellation

```tsx
// Error response structure
{
  success: false,
  domainException?: { errorDescription: string },
  statusCode: number,
  errors: Dictionary<string, string>
}
```
