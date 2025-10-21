using System.Reflection;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FarmsManager.Infrastructure;

public class FarmsManagerContext : DbContext, IUnitOfWork
{
    public FarmsManagerContext(DbContextOptions<FarmsManagerContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("farms_manager");
        var entities = typeof(UserEntity).Assembly.GetTypes().Where(t => t.IsAbstract == false)
            .Where(t => t.IsSubclassOf(typeof(Entity)));

        foreach (var entity in entities)
        {
            builder.Entity(entity);
            foreach (var propertyInfo in entity.GetProperties())
            {
                if (propertyInfo.PropertyType.IsEnum)
                {
                    var flagsAttribute = propertyInfo.PropertyType.GetCustomAttribute<FlagsAttribute>();
                    if (flagsAttribute == null)
                    {
                        builder.Entity(entity).Property(propertyInfo.Name).HasConversion<string>().HasMaxLength(100);
                    }
                    else
                    {
                        builder.Entity(entity).Property(propertyInfo.Name).HasConversion<long>();
                    }
                }
            }
        }


        builder.ApplyConfigurationsFromAssembly(typeof(FarmsManagerContext).Assembly);

        // Configure audit field relationships for all entities except UserEntity
        foreach (var entity in entities.Where(e => e != typeof(UserEntity)))
        {
            var entityBuilder = builder.Entity(entity);
            
            entityBuilder.HasOne(typeof(UserEntity), "Creator")
                .WithMany()
                .HasForeignKey("CreatedBy")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entityBuilder.HasOne(typeof(UserEntity), "Modifier")
                .WithMany()
                .HasForeignKey("ModifiedBy")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entityBuilder.HasOne(typeof(UserEntity), "Deleter")
                .WithMany()
                .HasForeignKey("DeletedBy")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }

        foreach (var type in builder.Model.GetEntityTypes())
        {
            var p = type.FindPrimaryKey()?.Properties.FirstOrDefault(t => t.ValueGenerated != ValueGenerated.Never);

            if (p != null)
            {
                p.ValueGenerated = ValueGenerated.Never;
            }
        }
    }
}