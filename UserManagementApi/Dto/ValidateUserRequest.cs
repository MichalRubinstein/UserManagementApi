using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Dto;

public record ValidateUserRequest(
    [Required] string UserName,
    [Required] string UserPassword);
