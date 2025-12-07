using Microsoft.EntityFrameworkCore;
using UserCore.Data;
using UserCore.DTOs;
using UserCore.Entities;
using UserCore.Interfaces.Repositories;

namespace UserCore.Repositories;

public class UserRepository(DataContext context) : IUserRepository
{
    public async Task Add(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetByEmail(string email)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email) ??
                   throw new Exception();
        return user;
    }

    public async Task<User> GetById(Guid id)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id) ??
                   throw new Exception();
        return user;
    }

    public async Task Update(User updateUser)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == updateUser.Id);
        if (user != null)
        {
            context.Users.Update(updateUser);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new Exception();
        }
    }

    public async Task Delete(Guid id)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id) ??
                   throw new Exception();
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> AddRegisteredObject(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            user.RegisteredObjects++;
            await context.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new Exception();
            return false;
        }

        return false;
    }
}