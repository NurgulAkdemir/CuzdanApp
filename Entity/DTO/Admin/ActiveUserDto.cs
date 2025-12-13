namespace Entity.DTO.Admin
{
    public class ActiveUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TransactionCount { get; set; }
        public decimal TotalTransactionAmount { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public bool IsActive { get; set; }
    }
}
