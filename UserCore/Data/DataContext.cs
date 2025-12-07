using Microsoft.EntityFrameworkCore;
using UserCore.Entities;

namespace UserCore.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}