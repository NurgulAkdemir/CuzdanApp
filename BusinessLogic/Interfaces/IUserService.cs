using Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Entity.DTO.User;


namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(UserCreateDto dto);
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<UserResponseDto> AuthenticateAsync(UserLoginDto dto);
        Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<UserResponseDto> UpdateEmailAsync(int userId, UpdateEmailDto dto);
        Task<IEnumerable<UserResponseDto>> GetAdminsAsync();
        Task<bool> SetBanStatusAsync(int userId, bool isBanned);
        Task<(IEnumerable<UserResponseDto> Items, int TotalCount)> QueryUsersAsync(
            string? search,
            string? role,
            bool? isBanned,
            string? sortBy,
            bool sortDesc,
            int page,
            int pageSize);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    }
}
