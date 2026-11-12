using Microsoft.AspNetCore.Identity;
using UserManagementApi.Business.Results;
using UserManagementApi.Dto;
using UserManagementApi.Models;
using UserManagementApi.Repositories;

namespace UserManagementApi.Business;

public class UserService(
    IUserRepository repository,
    IPasswordHasher<User> hasher,
    ILogger<UserService> logger) : IUserService
{

    public async Task<UserOperationResult<UserDetails>> CreateUserAsync(
        CreateUserRequest request, CancellationToken ct = default)
    {

        if (await repository.GetByUserNameAsync(request.UserName, ct) is not null)
        {
            logger.LogWarning("Attempt to create duplicate User {UserName}", request.UserName);
            return UserOperationResult<UserDetails>.Conflict();
        }

        var user = new User { UserName = request.UserName };
        user.PasswordHash = hasher.HashPassword(user, request.UserPassword);
        var addResult = await repository.AddAsync(user, ct);

        return addResult.Created switch
        {
            UserOutcome.Created => UserOperationResult<UserDetails>.Success(
                                new UserDetails(addResult.User!.UserId, addResult.User!.UserName)),
            UserOutcome.DuplicateUserName => UserOperationResult<UserDetails>.Conflict(),
            _ => UserOperationResult<UserDetails>.Conflict()
        };
    }

    public async Task<UserOperationResult<bool>> DeleteUserAsync(Guid userId, CancellationToken ct = default)
    {
        var deleted = await repository.DeleteAsync(userId, ct);
        return deleted
                ? UserOperationResult<bool>.Success(true)
                            : UserOperationResult<bool>.NotFound();
    }

    public async Task<UserOperationResult<UserDetails>> ValidateUserAsync(
                ValidateUserRequest request, CancellationToken ct = default)
    {
        var user = await repository.GetByUserNameAsync(request.UserName, ct);
        if (user is null)
            return UserOperationResult<UserDetails>.InvalidCredentials();
        var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, request.UserPassword);
        if (verification == PasswordVerificationResult.Failed)
            return UserOperationResult<UserDetails>.InvalidCredentials();
        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = hasher.HashPassword(user, request.UserPassword);
            await repository.UpdateAsync(user, ct);
        }
        return UserOperationResult<UserDetails>.Success(new UserDetails(user.UserId, user.UserName));
    }
}
