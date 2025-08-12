using DAL.Repositories;
using Entity;
using Entity.DTO;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto dto)
        {
            // 1. Basit validasyon
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email boş olamaz.");
            if (string.IsNullOrWhiteSpace(dto.Password)) throw new ArgumentException("Şifre boş olamaz.");

            // 2. Var mı kontrol et (unique email)
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Bu e-posta ile kayıtlı bir kullanıcı zaten var.");

            // 3. Parola hash'le
            string hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 4. Entity oluştur
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = hashed // entity'nde bu property yoksa ekle: public string PasswordHash {get; set;}
            };

            // 5. Kaydet
            await _userRepository.AddAsync(user);

            // 6. Dönüş DTO
            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<UserResponseDto> AuthenticateAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            bool ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            // güncellenecek alanlar
            if (!string.IsNullOrWhiteSpace(dto.UserName))
                user.UserName = dto.UserName;

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                // email unique kontrolü
                var other = await _userRepository.GetByEmailAsync(dto.Email);
                if (other != null && other.Id != user.Id)
                    throw new InvalidOperationException("Bu e-posta zaten kullanılıyor.");

                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _userRepository.UpdateAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }
    }
}
