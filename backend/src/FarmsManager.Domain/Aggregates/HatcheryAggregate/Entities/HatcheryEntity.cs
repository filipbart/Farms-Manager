﻿using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

public class HatcheryEntity : Entity
{
    protected HatcheryEntity()
    {
    }

    public string Name { get; protected internal set; }
    public string FullName { get; protected internal set; }
    public string Nip { get; protected internal set; }
    public string Address { get; protected internal set; }
    public string ProducerNumber { get; protected internal set; }

    public static HatcheryEntity CreateNew(string name, string producerNumber, string fullName, string nip,
        string address, Guid? userId = null)
    {
        return new HatcheryEntity
        {
            Name = name,
            ProducerNumber = producerNumber,
            FullName = fullName,
            Nip = nip.Replace("-", ""),
            Address = address,
            CreatedBy = userId
        };
    }
}