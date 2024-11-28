namespace System.Domain.Features.Identity;

public interface IUserRepository
{
    Task AddUser(UserM user);

    Task<UserM> GetUserById(Guid userId);

    Task<UserM?> GetUserByEmail(string userEmail);

    //Task<GetUserByIdDto> GetUserWithRoles(string userEmail);

    void UpdateUser(UserM user);

    //Task UpdateUserTokens(GetUserByIdDto userDto);

    Task SaveChangesAsync();
}
