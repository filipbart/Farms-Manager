using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure;

public class FarmsManagerContext : DbContext
{
    public FarmsManagerContext(DbContextOptions<FarmsManagerContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(FarmsManagerContext).Assembly);
        base.OnModelCreating(builder);
    }
}