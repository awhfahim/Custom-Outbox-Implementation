using System.Text.Json;
using MtslErp.Common.Domain.Entities;

namespace PrintFactoryManagement.Application.Features.PrintFactoryFeatures.Services;

public class PrintFactoryService : IPrintFactoryService
{
    private readonly IPrintFactoryAppUnitOfWork _unitOfWork;

    public PrintFactoryService(IPrintFactoryAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task CreateAsync()
    {
        return Task.CompletedTask;
    }
}
