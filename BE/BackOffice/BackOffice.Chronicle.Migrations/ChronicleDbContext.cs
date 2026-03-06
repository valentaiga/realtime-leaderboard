using BackOffice.Chronicle.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Chronicle.Migrations;

/// <summary>
/// DbContext used for migrations only. Since entity framework doesnt provide full NativeAOT support.
/// </summary>
public class ChronicleDbContext(DbContextOptions<ChronicleDbContext> options) : DbContext(options)
{
    public DbSet<MatchDto> Matches { get; set; }
    public DbSet<MatchPlayerDto> MatchPlayers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MatchDto>(e =>
        {
            e.ToTable(MatchDto.TableName);
            e.HasKey(x => x.Id);

            e.HasIndex(x => x.MatchId);
            e.HasIndex(x => x.StartedAt);
            e.HasIndex(x => x.FinishedAt);

            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
            e.Property(x => x.MatchId).HasColumnName("match_id").IsRequired().ValueGeneratedNever();
            e.Property(x => x.StartedAt).HasColumnName("started_at").IsRequired();
            e.Property(x => x.FinishedAt).HasColumnName("finished_at").IsRequired();
        });

        modelBuilder.Entity<MatchPlayerDto>(e =>
        {
            e.ToTable(MatchPlayerDto.TableName);
            e.HasKey(x => new { x.PlayerId, x.MatchId });
            
            e.HasIndex(x => x.PlayerId);
            e.HasIndex(x => new { x.PlayerId, x.IsWin });

            e.Property(x => x.MatchId).HasColumnName("match_id").IsRequired().ValueGeneratedNever();
            e.Property(x => x.PlayerId).HasColumnName("player_id").IsRequired();
            e.Property(x => x.IsWin).HasColumnName("is_win").IsRequired();

            e.HasOne<MatchDto>()
                .WithMany(x => x.Players)
                .HasForeignKey(x => x.MatchId)
                .HasPrincipalKey(x => x.Id);
        });
    }
}
