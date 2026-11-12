

using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using UserManagementApi.Data;
using UserManagementApi.Dto;
using UserManagementApi.Models;

namespace UserManagementApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.UserId == userId, ct);

    public Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);

    public async Task<AddUserResult> AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        try
        {
            await _db.SaveChangesAsync(ct);
            return new AddUserResult(UserOutcome.Created, user);
        }
        catch (DbUpdateException)
        {
            _db.Entry(user).State = EntityState.Detached;
            return new AddUserResult(UserOutcome.DuplicateUserName,user);
        }
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(userId, ct);
        if (user is null) return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
