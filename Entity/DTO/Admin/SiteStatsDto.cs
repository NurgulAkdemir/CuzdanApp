namespace Entity.DTO.Admin
{
    public class SiteStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int BannedUsers { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalTransactionVolume { get; set; }
        public int TotalInstallments { get; set; }
        public decimal TotalInstallmentAmount { get; set; }
        public decimal TotalRemainingInstallments { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
