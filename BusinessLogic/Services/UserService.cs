using DAL.Repositories;
using Entity;
using Entity.DTO;
using System;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Entity.DTO.User;

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
            if (string.IsNullOrWhiteSpace(dto.FirstName)) throw new ArgumentException("Ad boş olamaz.");
            if (string.IsNullOrWhiteSpace(dto.LastName)) throw new ArgumentException("Soyad boş olamaz.");
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email boş olamaz.");
            if (string.IsNullOrWhiteSpace(dto.PasswordHash)) throw new ArgumentException("Şifre boş olamaz.");

            // 2. Var mı kontrol et (unique email)
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Bu e-posta ile kayıtlı bir kullanıcı zaten var.");

            // 3. Parola hash'le
            string hashed = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);

            // 4. Entity oluştur
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = hashed
            };

            // 5. Kaydet
            await _userRepository.AddAsync(user);

            // 6. Dönüş DTO
            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.FullName,
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
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.FullName,
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
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.FullName,
                Email = user.Email
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            // güncellenecek alanlar
            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                // email unique kontrolü
                var other = await _userRepository.GetByEmailAsync(dto.Email);
                if (other != null && other.Id != user.Id)
                    throw new InvalidOperationException("Bu e-posta zaten kullanılıyor.");

                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);
            }

            await _userRepository.UpdateAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.FullName,
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
