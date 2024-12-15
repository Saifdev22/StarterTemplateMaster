﻿using Common.Domain.Abstractions;
using System.Domain.Identity;

namespace System.Application.Features.Users.DeleteUser;

internal sealed class DeleteUserHandler(IGenericRepository<UserM> _repository)
        : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteById(request.Request);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}