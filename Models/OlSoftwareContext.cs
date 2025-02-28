using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BackOlSoftware.Models;

public partial class OlSoftwareContext : DbContext
{
    public OlSoftwareContext()
    {
    }

    public OlSoftwareContext(DbContextOptions<OlSoftwareContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Establishment> Establishments { get; set; }

    public virtual DbSet<Merchant> Merchants { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<MerchantReport> MerchantReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Establishment>(entity =>
        {
            entity.HasKey(e => e.EstablishmentId).HasName("PK__Establis__3DB378E9D4A844FB");

            entity.ToTable(tb => tb.HasTrigger("trg_Establishment_Audit"));

            entity.Property(e => e.EstablishmentId).HasColumnName("EstablishmentID");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Revenue).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Merchant>(entity =>
        {
            entity.HasKey(e => e.MerchantId).HasName("PK__Merchant__0441656372D96317");

            entity.ToTable(tb => tb.HasTrigger("trg_Merchant_Audit"));

            entity.Property(e => e.MerchantId).HasColumnName("MerchantID");
            entity.Property(e => e.BusinessName).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasMany(d => d.Establishments).WithMany(p => p.Merchants)
                .UsingEntity<Dictionary<string, object>>(
                    "MerchantEstablishment",
                    r => r.HasOne<Establishment>().WithMany()
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Merchant___Estab__45F365D3"),
                    l => l.HasOne<Merchant>().WithMany()
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Merchant___Merch__44FF419A"),
                    j =>
                    {
                        j.HasKey("MerchantId", "EstablishmentId").HasName("PK__Merchant__879A52EDB6F67728");
                        j.ToTable("Merchant_Establishment");
                        j.IndexerProperty<int>("MerchantId").HasColumnName("MerchantID");
                        j.IndexerProperty<int>("EstablishmentId").HasColumnName("EstablishmentID");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3ADD5B2FCB");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F643BFB76F").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACF0EA4E34");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D2330AA1").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<MerchantReport>().HasNoKey();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
