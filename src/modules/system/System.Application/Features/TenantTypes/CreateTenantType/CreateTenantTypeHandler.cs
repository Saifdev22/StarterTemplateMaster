using Common.Domain.Abstractions;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantTypes.CreateTenantType;

internal sealed class CreateTenantTypeHandler(IGenericRepository<TenantTypeM> _repository)
        : ICommandHandler<CreateTenantTypeCommand, TenantTypeM>
{
    public async Task<Result<TenantTypeM>> Handle(CreateTenantTypeCommand request, CancellationToken cancellationToken)
    {
        TenantTypeM newTenantType = TenantTypeM.Create(
            request.Request.TenantTypeCode,
            request.Request.TenantTypeName);

        await _repository.AddAsync(newTenantType);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(newTenantType);
    }
}
