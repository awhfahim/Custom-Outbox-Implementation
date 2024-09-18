using PrintFactoryManagement.Domain.Orders;

namespace PrintFactoryManagement.Application.Features.PrintFactoryFeatures.Services;

public interface IPrintFactoryService
{
    Task CreateAsync(Order order);
}
