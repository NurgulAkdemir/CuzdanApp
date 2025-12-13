using Entity.DTO.Admin;

namespace UI.Models
{
    public class AdminStatsViewModel
    {
        public SiteStatsDto SiteStats { get; set; } = new();
        public List<TopTransactionCategoryDto> TopTransactionCategories { get; set; } = new();
        public List<ActiveUserDto> ActiveUsers { get; set; } = new();
        public TransactionVolumeDto TransactionVolume { get; set; } = new();
    }
}
