namespace Entity.DTO
{
    public class InstallmentUpdateDto
    {
        public decimal TotalAmount { get; set; }
        public int Months { get; set; }
        public decimal InterestRate { get; set; }
    }
}

