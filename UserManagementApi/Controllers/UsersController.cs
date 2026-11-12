using global::UserManagementApi.Business;
using global::UserManagementApi.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementApi.Controllers;

[ApiController]

[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType<UserDetails>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<Results<Created<UserDetails>, Conflict<ProblemDetails>, ProblemHttpResult>> CreateUser(
        [FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await userService.CreateUserAsync(request, ct);

        return result.Status switch
        {
            UserOperationStatus.Success => TypedResults.Created($"/api/users/{result.Value!.UserId}", result.Value),
            UserOperationStatus.Conflict => TypedResults.Conflict(
                                new ProblemDetails { Title = "User name already exists", Status = StatusCodes.Status409Conflict }),
            _ => TypedResults.Problem(MapError(result.Status))
        };
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<Results<NoContent, NotFound<ProblemDetails>, ProblemHttpResult>> DeleteUser(
    Guid userId, CancellationToken ct)
    {
        var result = await userService.DeleteUserAsync(userId, ct);
        return result.Status switch
        {
            UserOperationStatus.Success => TypedResults.NoContent(),
            UserOperationStatus.NotFound => TypedResults.NotFound(
                                new ProblemDetails { Title = "User not found", Status = StatusCodes.Status404NotFound }),
            _ => TypedResults.Problem(MapError(result.Status))
        };
    }
    [HttpPost("validate")]
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<Results<Ok<UserDetails>, UnauthorizedHttpResult, ProblemHttpResult>> ValidateUser(
                [FromBody] ValidateUserRequest request, CancellationToken ct)
    {
        var result = await userService.ValidateUserAsync(request, ct);
        return result.Status switch
        {
            UserOperationStatus.Success => TypedResults.Ok(result.Value!),
            UserOperationStatus.InvalidCredentials => TypedResults.Unauthorized(),
            _ => TypedResults.Problem(MapError(result.Status))
        };
    }
    private static ProblemDetails MapError(UserOperationStatus status) => new()
    {
        Title = "Unexpected error",
        Detail = "The operation could not be completed.",
        Status = StatusCodes.Status500InternalServerError,
        Extensions = { ["operationStatus"] = status.ToString() }
    };
}
