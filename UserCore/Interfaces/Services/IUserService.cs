using UserCore.DTOs;
using UserCore.Entities;

namespace UserCore.Interfaces.Services;

public interface IUserService
{
    Task Register(RegisterDto registerDto);
    Task<UserResponse> Login(LoginDto login);
    Task<User> GetById(Guid id);
    Task Update(User updateUser);
    Task Delete(Guid id);
    Task<bool> AddRegisteredObject(Guid id);
}