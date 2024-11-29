using Common.Domain.Results;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Domain.Features.Identity;
using System.Infrastructure.Common.Database;

namespace System.Infrastructure.Features.Identity;

internal sealed class UserRepository(SystemDbContext _dbContext) : IIdentityRepository
{
    public async Task AddUser(UserM user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<UserM> GetUserById(Guid userId)
    {
        UserM? user = await _dbContext.Users.FindAsync(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
        return user;
    }



    public async Task<UserM?> GetUserByEmail(string userEmail)
    {
        UserM? user = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == userEmail);
        return user;
    }

    public void UpdateUser(UserM user)
    {
        _dbContext.Users.Update(user);
    }

    public async Task<GetUserByIdDto> GetUserWithRoles(string userEmail)
    {
        GetUserByIdDto user = await _dbContext.Users
                    .Include(p => p.UserRoles!)  // Ensure UserRoles is included
                        .ThenInclude(c => c.Role)
                    .Where(e => e.Email == userEmail)
                    .Select(s => new GetUserByIdDto
                    {
                        UserId = s.UserId,
                        TenantId = s.TenantId,
                        Email = s.Email,
                        PasswordHash = s.PasswordHash,
                        PasswordSalt = s.PasswordSalt,
                        RefreshToken = s.RefreshToken,
                        RefreshTokenExpiryTime = s.RefreshTokenExpiryTime,
                    })
                    .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"User with email {userEmail} not found.");
        return user;
    }


    public async Task UpdateUserTokens(GetUserByIdDto userDto)
    {
        UserM? user = await _dbContext.Users.FindAsync(userDto.UserId);

        if (user != null)
        {
            user.RefreshToken = userDto.RefreshToken!;
            user.RefreshTokenExpiryTime = userDto.RefreshTokenExpiryTime;

            _dbContext.Update(user);
        }
        else
        {
            Result.Failure(IdentityErrors.NotFound(userDto.UserId));
        }

    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
