using Entity.DTO;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(UserCreateDto dto);
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<UserResponseDto> AuthenticateAsync(UserLoginDto dto);
        Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int id);
    }
}
