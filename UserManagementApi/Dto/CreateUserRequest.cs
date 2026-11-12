using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Dto;

public record CreateUserRequest(
    [Required, MinLength(3), MaxLength(100)] string UserName,
    [Required, MinLength(6)] string UserPassword);
