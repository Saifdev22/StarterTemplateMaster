using Common.Domain.Errors;
using System.Domain.Features.Identity;

namespace System.Application.Features.Users.CreateUser;

internal sealed class CreateUserHandler(IUserRepository _userRepository)
        : ICommandHandler<CreateUserCommand, int>
{
    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        UserM? user = await _userRepository.GetUserByEmail(request.Request.Email);
        if (user is not null)
        {
            return Result.Failure<int>(CustomError.Conflict("406", "Email already exists."));
        }

        UserM newUser = UserM.Create(
            request.Request.TenantId,
            request.Request.Email,
            request.Request.Password);

        await _userRepository.AddUser(newUser);
        await _userRepository.SaveChangesAsync();

        return Result.Success(newUser.UserId);
    }
}
