using System;

namespace Entity
{
    public class Balance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = "Bakiye ekleme";

        public User? User { get; set; }

    }
}
