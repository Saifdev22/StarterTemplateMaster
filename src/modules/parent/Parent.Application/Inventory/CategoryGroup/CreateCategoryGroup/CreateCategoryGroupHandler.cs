﻿using Common.Application.Messaging;
using Common.Domain.Results;
using Parent.Application.Common;
using Parent.Domain.Inventory.CategoryGroup;

namespace Parent.Application.Inventory.CategoryGroup.CreateCategoryGroup;

internal sealed class CreateCategoryGroupHandler(IGenericRepository2<CategoryGroupM> _repository)
        : ICommandHandler<CreateCategoryGroupCommand, CategoryGroupM>
{
    public async Task<Result<CategoryGroupM>> Handle(CreateCategoryGroupCommand request, CancellationToken cancellationToken)
    {
        CategoryGroupM obj = CategoryGroupM.Create
        (
            request.Request.CategoryGroupCode,
            request.Request.CategoryGroupDesc
        );

        await _repository.AddAsync(obj);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(obj);
    }
}