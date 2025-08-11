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
            // İlişkiler ve özel tablolar için Fluent API ayarları buraya gelecek
        }
    }
}
