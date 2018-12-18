namespace PB.WebAPI.Services
{
    public interface ITokenService
    {
        string GenerateToken(long userID);
    }
}