namespace FarmsManager.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelAttribute : Attribute
{
    public ExcelAttribute(string columnName)
    {
        ColumnName = columnName;
    }

    public string ColumnName { get; }
    public string CellFormat { get; set; }
    public bool IsCurrency { get; set; }
    public bool IsList { get; set; }
}