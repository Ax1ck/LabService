namespace UserCore.DTOs;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
}