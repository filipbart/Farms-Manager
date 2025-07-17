namespace FarmsManager.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class AzureDiFieldAttribute : Attribute
{
    public string FieldName { get; }
    public bool CustomField { get; }

    public AzureDiFieldAttribute(string fieldName, bool customField = false)
    {
        FieldName = fieldName;
        CustomField = customField;
    }
}