---
# Consistent HTTP Methods Pattern

Standardized HTTP method usage across services:

```tsx
export class FarmsService {
  // Read operations
  public static async getFarmsAsync() {
    return await AxiosWrapper.get<PaginateModel<FarmRowModel>>(ApiUrl.Farms);
  }

  // Create operations
  public static async addFarmAsync(data: AddFarmFormData) {
    return await AxiosWrapper.post(ApiUrl.AddFarm, data);
  }

  // Update operations (partial)
  public static async updateFarmAsync(id: string, data: FarmFormValues) {
    return await AxiosWrapper.patch(ApiUrl.UpdateFarm(id), data);
  }

  // Delete operations (usually POST, some DELETE)
  public static async deleteFarmAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteFarm(id));  // Standard
  }

  // PUT rarely used, mostly for full replacements
}
```

- **GET** - All read operations
- **POST** - Create operations and most delete operations
- **PATCH** - Partial updates (standard for updates)
- **DELETE** - Some delete operations (exceptions exist)
- **PUT** - Rarely used, mostly for full replacements
- **Consistency** - Follow backend routing conventions

```tsx
// Backend expects POST for delete
POST /api/farms/delete/{id}
// But some endpoints use DELETE
DELETE /api/users/{id}/delete
```
