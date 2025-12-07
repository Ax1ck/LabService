namespace UserCore.Interfaces.Auth;

public interface IPasswordHasher
{
    string Generate(string password);
    public bool Verify(string password, string hashedPassword);
}