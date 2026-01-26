---
# ApiUrl Constants Pattern

Centralized API endpoint definitions:

```tsx
export default class ApiUrl {
  private static BaseUrl = import.meta.env.PROD
    ? import.meta.env.VITE_API_BASE_URL
    : "/api/";

  public static Farms = this.BaseUrl + "farms";
  public static AddFarm = this.Farms + "/add";
  public static UpdateFarm = (id: string) => this.Farms + "/update/" + id;
  public static DeleteFarm = (id: string) => this.Farms + "/delete/" + id;
}
```

- **All Endpoints Centralized** - All API URLs must be in ApiUrl class, no inline URLs
- **Environment-Based Base URL** - Production uses full URL, development uses "/api/" proxy
- **Functional URLs** - Use functions for dynamic URLs (UpdateFarm(id), DeleteFarm(id))
- **Consistent Naming** - Follow REST conventions: AddXxx, UpdateXxx(id), DeleteXxx(id)
- **Hierarchical Organization** - Group related endpoints under base resources

```tsx
// Usage in services
return await AxiosWrapper.post(ApiUrl.AddFarm, data);
return await AxiosWrapper.patch(ApiUrl.UpdateFarm(id), data);
```
