using UserCore.DTOs;
using UserCore.Entities;
using UserCore.Interfaces.Auth;
using UserCore.Interfaces.Repositories;
using UserCore.Interfaces.Services;

namespace UserCore.Services;

public class UserService(
    IPasswordHasher passwordHasher, 
    IUserRepository userRepository, 
    IJwtProvider jwtProvider) : IUserService
{
    public async Task Register(RegisterDto registerDto)
    {
        var hashedPassword = passwordHasher.Generate(registerDto.Password);
        
        User user = new User
        {
            Email = registerDto.Email,
            Password = hashedPassword,
            Name = registerDto.Name,
            Phone = registerDto.Phone
        };

        await userRepository.Add(user);
    }

    public async Task<UserResponse> Login(LoginDto login)
    {
        var user = await userRepository.GetByEmail(login.Email);
        var result = passwordHasher.Verify(login.Password, user.Password);
        if (!result)
            throw new Exception();
        var userResponse = new UserResponse
        {
            Id = user.Id,
            Token = jwtProvider.GenerateToken(user)
        };
        return userResponse;
    }

    public async Task<User> GetById(Guid id)
    {
        var user = await userRepository.GetById(id);
        return user;
    }

    public async Task Update(User updateUser)
    {
        await userRepository.Update(updateUser);
    }

    public async Task Delete(Guid id)
    {
        await userRepository.Delete(id);
    }

    public async Task<bool> AddRegisteredObject(Guid id)
    {
        try
        {
            Console.WriteLine($"Работает! id = {id}");
            return await userRepository.AddRegisteredObject(id);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        return true;
    }
}