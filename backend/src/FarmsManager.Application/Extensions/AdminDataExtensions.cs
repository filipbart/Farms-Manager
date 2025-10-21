namespace FarmsManager.Application.Extensions;

public static class AdminDataExtensions
{
    public static T ClearAdminData<T>(this T dto, bool isAdmin) where T : class
    {
        if (isAdmin) return dto;

        var type = typeof(T);
        
        var createdByNameProp = type.GetProperty("CreatedByName");
        if (createdByNameProp != null && createdByNameProp.CanWrite)
        {
            createdByNameProp.SetValue(dto, null);
        }

        var modifiedByNameProp = type.GetProperty("ModifiedByName");
        if (modifiedByNameProp != null && modifiedByNameProp.CanWrite)
        {
            modifiedByNameProp.SetValue(dto, null);
        }

        var deletedByNameProp = type.GetProperty("DeletedByName");
        if (deletedByNameProp != null && deletedByNameProp.CanWrite)
        {
            deletedByNameProp.SetValue(dto, null);
        }

        var dateModifiedUtcProp = type.GetProperty("DateModifiedUtc");
        if (dateModifiedUtcProp != null && dateModifiedUtcProp.CanWrite)
        {
            dateModifiedUtcProp.SetValue(dto, null);
        }

        var dateDeletedUtcProp = type.GetProperty("DateDeletedUtc");
        if (dateDeletedUtcProp != null && dateDeletedUtcProp.CanWrite)
        {
            dateDeletedUtcProp.SetValue(dto, null);
        }

        return dto;
    }

    public static IEnumerable<T> ClearAdminData<T>(this IEnumerable<T> dtos, bool isAdmin) where T : class
    {
        if (isAdmin) return dtos;
        
        return dtos.Select(dto => dto.ClearAdminData(isAdmin)).ToList();
    }
}
