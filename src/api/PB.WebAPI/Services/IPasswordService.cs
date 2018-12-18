namespace PB.WebAPI.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool ValidatePassword(string password, string correctHash);
    }
}