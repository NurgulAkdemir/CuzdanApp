using System;

namespace Entity
{
    public class Balance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime AsOf { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }

    }
}
