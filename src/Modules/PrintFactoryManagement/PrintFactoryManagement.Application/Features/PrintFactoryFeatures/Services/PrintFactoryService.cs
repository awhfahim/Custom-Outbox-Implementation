using System.Text.Json;
using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Domain.Events;
using PrintFactoryManagement.Domain.Orders;

namespace PrintFactoryManagement.Application.Features.PrintFactoryFeatures.Services;

public class PrintFactoryService : IPrintFactoryService
{
    private readonly IPrintFactoryAppUnitOfWork _unitOfWork;

    public PrintFactoryService(IPrintFactoryAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(Order order)
    {
        await _unitOfWork.OrderRepository.CreateAsync(order);

        var orderCreatedEvent = new OrderCreatedEvent()
        {
            Name = order.Name, Description = order.Description, CreatedOnUtc = order.CreatedOnUtc
        };

        var orderCreatedMessage = JsonSerializer.Serialize(orderCreatedEvent);

        var outbox = new OutboxMessage
        {
            Payload = orderCreatedMessage,
            Status = true,
            CreatedOn = DateTime.Now,
            PayloadType = typeof(OrderCreatedEvent).AssemblyQualifiedName ?? nameof(OrderCreatedEvent)
        };

        await _unitOfWork.PfmOutboxRepository.CreateAsync(outbox);

        await _unitOfWork.SaveAsync();
    }
}
