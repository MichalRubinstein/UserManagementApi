using UserManagementApi.Dto;

namespace UserManagementApi.Business.Results;

public sealed record UserOperationResult<T>(UserOperationStatus Status, T? Value = default)
{
    public static UserOperationResult<T> Success(T value) => new(UserOperationStatus.Success, value);
    public static UserOperationResult<T> NotFound() => new(UserOperationStatus.NotFound);
    public static UserOperationResult<T> Conflict() => new(UserOperationStatus.Conflict);
    public static UserOperationResult<T> InvalidCredentials() => new(UserOperationStatus.InvalidCredentials);
}