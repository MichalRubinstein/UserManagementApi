using UserManagementApi.Business.Results;
using UserManagementApi.Dto;

namespace UserManagementApi.Business;

public interface IUserService
{
    Task<UserOperationResult<UserDetails>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserOperationResult<bool>> DeleteUserAsync(Guid userId, CancellationToken ct = default);
    Task<UserOperationResult<UserDetails>> ValidateUserAsync(ValidateUserRequest request, CancellationToken ct = default);
}
