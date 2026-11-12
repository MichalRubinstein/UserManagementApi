using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementApi.Models;

public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required, Column("UserPassword")]
    public string PasswordHash { get; set; } = string.Empty;
}
