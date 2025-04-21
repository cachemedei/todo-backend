namespace backend.Contracts;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string FirstName { get; set; }
    public int AccountId { get; set; }
}