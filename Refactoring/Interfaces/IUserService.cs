public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> EditAsync(UserUpdate request, Guid id);

}