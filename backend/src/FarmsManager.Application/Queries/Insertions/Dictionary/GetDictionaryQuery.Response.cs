namespace FarmsManager.Application.Queries.Insertions.Dictionary;

public class GetDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<HatcheryDictModel> Hatcheries { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}

public class FarmDictModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<HenhouseDictModel> Henhouses { get; set; } = [];
}

public class HenhouseDictModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class HatcheryDictModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class CycleDictModel
{
    public string Id { get; set; }
    public int Identifier { get; set; }
    public int Year { get; set; }
}