---
# Typed Service Methods Pattern

Explicit generics for type-safe API responses:

```tsx
export class FarmsService {
  // Simple type
  public static async getFarmsAsync() {
    return await AxiosWrapper.get<PaginateModel<FarmRowModel>>(ApiUrl.Farms);
  }

  // Complex type with generics
  public static async getFarmHousesAsync(farmId: string) {
    return await AxiosWrapper.get<PaginateModel<HouseRowModel>>(
      ApiUrl.Farms + "/" + farmId + "/henhouses"
    );
  }

  // Array type
  public static async getFarmCycles(farmId: string) {
    return await AxiosWrapper.get<CycleDto[]>(ApiUrl.GetCycles(farmId));
  }
}
```

- **Explicit Generics Preferred** - Use `<T>` for type safety, though some cases may omit
- **Case-Dependent Types** - Use simple types for simple responses, complex types (PaginateModel<T>) for lists/pagination
- **Type Safety** - Generics ensure response data matches expected interface
- **IntelliSense Support** - Explicit types provide better IDE support
- **Runtime Validation** - TypeScript validates types at compile time

```tsx
// Response type structure
BaseResponse<T> {
  success: boolean;
  responseData?: T;  // T is your specified type
  errors?: Dictionary<string, string>;
}
```
