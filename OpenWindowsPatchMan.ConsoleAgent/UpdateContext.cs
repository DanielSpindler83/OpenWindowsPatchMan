using Microsoft.EntityFrameworkCore;

namespace OpenWindowsPatchMan.ConsoleAgent;

public class UpdateContext : DbContext
{
    public UpdateContext(DbContextOptions<UpdateContext> options)
        : base(options)
    {
    }

    public DbSet<WindowsUpdateInfo> WindowsUpdateInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define the primary key explicitly
        modelBuilder.Entity<WindowsUpdateInfo>()
            .HasKey(e => e.WindowsUpdateInfoEntryId); // Make sure 'Id' is defined in your model

        // Convert List<string> to a single string and vice versa
        modelBuilder.Entity<WindowsUpdateInfo>()
            .Property(e => e.KBArticleIDs)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<WindowsUpdateInfo>()
            .Property(e => e.Categories)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<WindowsUpdateInfo>()
            .Property(e => e.UninstallationSteps)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<WindowsUpdateInfo>()
            .Property(e => e.SupersededUpdateIDs)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<WindowsUpdateInfo>()
            .Property(e => e.SecurityBulletinIDs)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<WindowsUpdateInfo>()
            .Property(e => e.BundledUpdates)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        // Map to the "WindowsUpdateInfo" table
        modelBuilder.Entity<WindowsUpdateInfo>().ToTable("WindowsUpdateInfo");
    }
}