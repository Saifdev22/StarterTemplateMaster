using Common.Domain.Abstractions;
using System.Domain.Identity;

namespace System.Application.Features.Users.UpdateUser;

internal sealed class UpdateUserHandler(IGenericRepository<UserM> _repository)
        : ICommandHandler<UpdateUserCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        UserM obj = await _repository.GetByIdAsync(request.Request.UserId);

        obj.Email = request.Request.Email;

        _repository.Update(obj);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}