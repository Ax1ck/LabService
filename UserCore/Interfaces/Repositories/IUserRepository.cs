using UserCore.DTOs;
using UserCore.Entities;

namespace UserCore.Interfaces.Repositories;

public interface IUserRepository
{
    Task Add(User user);
    Task<User> GetByEmail(string email);
    Task<User> GetById(Guid id);
    Task Update(User updateUser);
    Task Delete(Guid id);
    Task<bool> AddRegisteredObject(Guid id);
}