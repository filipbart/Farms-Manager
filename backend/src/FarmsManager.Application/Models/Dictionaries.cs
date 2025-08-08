namespace FarmsManager.Application.Models;

public class FarmDictModel : DictModel
{
    public List<DictModel> Henhouses { get; set; } = [];
}

public class DictModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class CycleDictModel
{
    public int Identifier { get; set; }
    public int Year { get; set; }
}