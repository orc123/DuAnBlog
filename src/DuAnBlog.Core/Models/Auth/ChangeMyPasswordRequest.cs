namespace DuAnBlog.Core.Models.Auth;
public class ChangeMyPasswordRequest
{
    public string OldPassword { get; set; }

    public string NewPassword { get; set; }
}
