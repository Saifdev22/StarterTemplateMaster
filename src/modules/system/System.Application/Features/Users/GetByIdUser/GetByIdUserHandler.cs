using Common.Domain.Abstractions;
using Common.Domain.TransferObjects.System;
using System.Domain.Identity;

namespace System.Application.Features.Users.GetByIdUser;

internal sealed class GetByIdUserHandler(IGenericRepository<UserM> genericRepository)
    : IQueryHandler<GetByIdUserQuery, ReadUserDto>
{
    public async Task<Result<ReadUserDto>> Handle(
        GetByIdUserQuery request,
        CancellationToken cancellationToken)
    {
        UserM obj = await genericRepository.GetByIdAsync(request.Request);

        ReadUserDto result = new
        (
            obj.UserId,
            obj.Email,
            obj.IsActive
        );

        return result;
    }
}