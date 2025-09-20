namespace Application.Interfaces;

public interface IUserService
{
    Task<bool> UserExistAsync(string userId);
}
