using Microsoft.EntityFrameworkCore;
using Entity; // Entity katmanındaki modelleri kullanmak için

namespace DAL.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Installment> Installments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // User - Transaction ilişkisi
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Installment ilişkisi
            modelBuilder.Entity<User>()
                .HasMany(u => u.Installments)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Balance ilişkisi
            modelBuilder.Entity<Balance>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Installment - Transaction ilişkisi
            modelBuilder.Entity<Installment>()
                .HasMany(i => i.Transactions)
                .WithOne(t => t.Installment)
                .HasForeignKey(t => t.InstallmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
