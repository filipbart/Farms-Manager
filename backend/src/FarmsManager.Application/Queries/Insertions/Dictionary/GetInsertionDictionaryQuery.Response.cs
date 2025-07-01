using FarmsManager.Application.Models;

namespace FarmsManager.Application.Queries.Insertions.Dictionary;

public class GetInsertionDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<DictModel> Hatcheries { get; set; } = [];
    public List<CycleDictModel> Cycles { get; set; } = [];
}