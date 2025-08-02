using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class FarmEntity : Entity
{
    protected FarmEntity()
    {
    }

    public string Name { get; protected internal set; }
    public string Nip { get; protected internal set; }
    public string Address { get; protected internal set; }
    public Guid? ActiveCycleId { get; protected internal set; }
    public string ProducerNumber { get; protected internal set; }
    public virtual CycleEntity ActiveCycle { get; protected internal set; }

    /// <summary>
    /// Kurniki
    /// </summary>
    private readonly List<HenhouseEntity> _henhouses = [];

    private readonly List<EmployeeEntity> _employees = [];

    public virtual IReadOnlyCollection<HenhouseEntity> Henhouses => _henhouses.AsReadOnly();
    public virtual IReadOnlyCollection<EmployeeEntity> Employees => _employees.AsReadOnly();

    /// <summary>
    /// Cykle
    /// </summary>
    private readonly List<CycleEntity> _cycles = [];

    public virtual IReadOnlyCollection<CycleEntity> Cycles => _cycles.AsReadOnly();

    public static FarmEntity CreateNew(string name, string producerNumber, string nip, string address,
        Guid? createdBy = null)
    {
        return new FarmEntity
        {
            Name = name,
            ProducerNumber = producerNumber,
            Nip = nip.Replace("-", ""),
            Address = address,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Ustawia aktywny cykl
    /// </summary>
    /// <param name="cycle"></param>
    public void SetLatestCycle(CycleEntity cycle)
    {
        _cycles.Add(cycle);
        ActiveCycle = cycle;
        ActiveCycleId = cycle.Id;
    }
}