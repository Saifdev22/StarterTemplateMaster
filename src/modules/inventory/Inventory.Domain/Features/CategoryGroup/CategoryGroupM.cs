using Common.Domain.Abstractions;

namespace Inventory.Domain.Features.CategoryGroup;

public sealed class CategoryGroupM : AggregateRoot
{
    public int CategoryGroupId { get; }
    public string CategoryGroupCode { get; private set; } = string.Empty;
    public string CategoryGroupDesc { get; private set; } = string.Empty;

    public static CategoryGroupM Create
    (
        string categoryGroupCode,
        string categoryGroupDesc
    )
    {
        CategoryGroupM categoryGroup = new()
        {
            CategoryGroupCode = categoryGroupCode,
            CategoryGroupDesc = categoryGroupDesc,
        };

        return categoryGroup;
    }

}