namespace FarmsManager.Shared.Attributes;

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