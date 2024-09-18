using MtslErp.Common.Domain.Interfaces;

namespace PrintFactoryManagement.Domain.Orders;

public sealed class Order : IEntity<int>
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }
}
