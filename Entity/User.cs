using System;
using System.Collections.Generic;

namespace Entity
{
    public enum UserRole { User = 0, Admin = 1 }

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!; 
        public string PasswordHash { get; set; } = null!; 

        public bool EmailConfirmed { get; set; } = false;
        public UserRole Role { get; set; } = UserRole.User;
        public decimal Balance { get; set; } = 0m; // isteðe baðlý; alternatif: hesaplamayla elde et
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<Transaction> Transactions { get; set; } = new();
        public List<Installment> Installments { get; set; } = new();
    }
}
