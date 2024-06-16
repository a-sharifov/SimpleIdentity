using System;
using System.Collections.Generic;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.DbContexts;

public partial class UserDbContext : DbContext
{
    public UserDbContext()
    {
    }

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

    public virtual DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlacklistedToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blacklis__3214EC074E81FFED");

            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmailConfirmationToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailCon__3214EC07697EEF82");

            entity.HasIndex(e => e.UserId, "UQ__EmailCon__1788CC4D0885BFD8").IsUnique();

            entity.HasOne(d => d.User).WithOne(p => p.EmailConfirmationToken)
                .HasForeignKey<EmailConfirmationToken>(d => d.UserId)
                .HasConstraintName("FK__EmailConf__UserI__2E1BDC42");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07769E9E97");

            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RefreshTo__UserI__30F848ED");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07F38B6D0F");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F6D33364D9").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC071117613E");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4252C346B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534811CA010").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__2A4B4B5E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
