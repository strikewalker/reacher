using Reacher.Data.Models;

namespace Reacher.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<DbReachable> Reachables { get; set; }
    public DbSet<DbEmail> Emails { get; set; }
    public DbSet<DbUser> Users { get; set; }
    public DbSet<DbWhitelist> Whitelist { get; set; }

    public DbSet<EnumTable<EmailType>> EmailTypes { get; set; }
    public DbSet<EnumTable<InvoiceStatus>> InvoiceStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EnumTable<EmailType>>().SeedEnumValues();
        modelBuilder.Entity<EnumTable<InvoiceStatus>>().SeedEnumValues();
    }

    #region Timestamp tracking
    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AddTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
        foreach (var entity in entities)
        {
            BaseModel record = ((BaseModel)entity.Entity);
            if (entity.State == EntityState.Added)
            {
                record.CreatedDate = DateTime.UtcNow;
            }
            else
            {
                Entry(record).Property(x => x.CreatedDate).IsModified = false;
            }
            record.ModifiedDate = DateTime.UtcNow;
        }
    }
    #endregion
}
