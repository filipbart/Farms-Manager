using System.Collections;
using System.Reflection;
using Azure.AI.DocumentIntelligence;
using FarmsManager.Shared.Attributes;

namespace FarmsManager.Shared.Extensions;

public static class AzureDiMappingExtensions
{
    public static T MapFromAzureDiFields<T>(this IReadOnlyDictionary<string, DocumentField> fields) where T : new()
    {
        var obj = new T();
        var type = typeof(T);

        foreach (var prop in type.GetProperties())
        {
            var attr = prop.GetCustomAttribute<AzureDiFieldAttribute>();
            if (attr == null) continue;

            if (fields.TryGetValue(attr.FieldName, out var fieldValue))
            {
                object value;


                if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) &&
                    fieldValue.FieldType == DocumentFieldType.List)
                {
                    var itemType = prop.PropertyType.GetGenericArguments()[0];
                    var listInstance = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));

                    foreach (var itemField in fieldValue.ValueList)
                    {
                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            var mappedItem = typeof(AzureDiMappingExtensions)
                                .GetMethod(nameof(MapFromAzureDiFields))
                                ?.MakeGenericMethod(itemType)
                                .Invoke(null, [itemField.ValueDictionary]);

                            listInstance?.Add(mappedItem);
                        }
                    }

                    value = listInstance;
                }
                else
                {
                    value = MapSingleFieldValue(prop.PropertyType, fieldValue);
                }

                if (value != null)
                {
                    prop.SetValue(obj, value);
                }
            }
        }

        return obj;
    }


    private static object MapSingleFieldValue(Type propType, DocumentField fieldValue)
    {
        if (fieldValue.FieldType == DocumentFieldType.String)
        {
            return fieldValue.ValueString;
        }

        if (fieldValue.FieldType == DocumentFieldType.Address)
        {
            return fieldValue.Content;
        }

        if (fieldValue.FieldType == DocumentFieldType.Date)
        {
            if (fieldValue.ValueDate.HasValue)
            {
                var date = fieldValue.ValueDate.Value.DateTime;
                if (propType == typeof(DateOnly) || propType == typeof(DateOnly?))
                {
                    return DateOnly.FromDateTime(date);
                }

                return date;
            }
        }
        else if (fieldValue.FieldType == DocumentFieldType.Currency)
        {
            var doubleValue = fieldValue.ValueCurrency.Amount;

            if (propType == typeof(decimal) || propType == typeof(decimal?))
            {
                return Convert.ToDecimal(doubleValue);
            }

            if (propType == typeof(double) || propType == typeof(double?))
            {
                return doubleValue;
            }
        }
        else if (fieldValue.FieldType == DocumentFieldType.Double)
        {
            var content = fieldValue.Content;

            if (content.Contains('\n'))
            {
                var value = content.Split('\n')[0];
                return decimal.Parse(value);
            }

            var doubleValue = fieldValue.ValueDouble;

            if (propType == typeof(decimal) || propType == typeof(decimal?))
            {
                return Convert.ToDecimal(doubleValue);
            }

            if (propType == typeof(double) || propType == typeof(double?))
            {
                return doubleValue;
            }
        }
        else if (fieldValue.FieldType == DocumentFieldType.Int64)
        {
            return fieldValue.ValueInt64;
        }

        return null;
    }
}