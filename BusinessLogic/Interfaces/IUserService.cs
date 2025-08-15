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
    }
}
