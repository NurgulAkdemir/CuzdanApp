namespace Entity.DTO
{
    public class UserUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Eğer şifre güncellenecekse
    }
}
