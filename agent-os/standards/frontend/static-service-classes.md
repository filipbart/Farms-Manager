---
# Static Service Classes Pattern

All API services are implemented as static classes:

```tsx
export class FarmsService {
  public static async getFarmsAsync() {
    return await AxiosWrapper.get<PaginateModel<FarmRowModel>>(ApiUrl.Farms);
  }

  public static async addFarmAsync(data: AddFarmFormData) {
    return await AxiosWrapper.post(ApiUrl.AddFarm, data);
  }

  public static async updateFarmAsync(id: string, data: FarmFormValues) {
    return await AxiosWrapper.patch(ApiUrl.UpdateFarm(id), data);
  }
}
```

- **Static Classes Only** - All services must be static, no instance-based services
- **No Interfaces** - Static classes without interfaces (simpler approach)
- **Method Naming** - Use async suffix for async methods (getFarmsAsync, addFarmAsync)
- **Typed Returns** - Explicit generics for return types
- **AxiosWrapper Usage** - All methods use AxiosWrapper for HTTP calls

```tsx
// Usage in components
const farms = await FarmsService.getFarmsAsync();
await FarmsService.addFarmAsync(formData);
```
