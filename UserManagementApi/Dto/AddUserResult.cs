using UserManagementApi.Models;

namespace UserManagementApi.Dto;

public class AddUserResult
{
    public UserOutcome Created { get; set; }
    public User User { get; set; }

    public AddUserResult(UserOutcome created, User user)
    {
        this.Created = created;
        this.User = user;
    }
}