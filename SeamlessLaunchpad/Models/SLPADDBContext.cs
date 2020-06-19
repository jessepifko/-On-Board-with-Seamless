using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SeamlessLaunchpad.Models
{
    public partial class SLPADDBContext : DbContext
    {
        public SLPADDBContext()
        {
        }

        public SLPADDBContext(DbContextOptions<SLPADDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Favorites> Favorites { get; set; }
        public virtual DbSet<Startup> Startup { get; set; }
        public virtual DbSet<UserView> UserView { get; set; }
        public virtual DbSet<ViewFilter> ViewFilter { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = new StreamReader(File.OpenRead("secret.txt"));
                optionsBuilder.UseSqlServer(connectionString.ReadToEnd());
                connectionString.Close();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Association).HasMaxLength(20);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Favorites>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Startup)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.StartupId)
                    .HasConstraintName("FK__Favorites__Start__60A75C0F");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Favorites__UserI__619B8048");
            });

            modelBuilder.Entity<Startup>(entity =>
            {
                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Comments).HasMaxLength(200);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.Property(e => e.DateRemoved).HasColumnType("date");

                entity.Property(e => e.InterestedPartners).HasMaxLength(120);

                entity.Property(e => e.Name).HasMaxLength(80);

                entity.Property(e => e.Status).HasMaxLength(15);

                entity.Property(e => e.Summary).HasMaxLength(200);

                entity.Property(e => e.TechArea).HasMaxLength(120);

                entity.Property(e => e.Theme).HasMaxLength(120);
            });

            modelBuilder.Entity<UserView>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserView)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserView__UserId__6477ECF3");
            });

            modelBuilder.Entity<ViewFilter>(entity =>
            {
                entity.Property(e => e.FilterName).HasMaxLength(50);

                entity.Property(e => e.FilterValue)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.View)
                    .WithMany(p => p.ViewFilter)
                    .HasForeignKey(d => d.ViewId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ViewFilte__ViewI__6754599E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
