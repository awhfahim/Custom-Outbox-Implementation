using System.Text.Json;
using Dapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using MtslErp.Common.Application.Data;
using MtslErp.Common.Domain.Entities;
using Quartz;

namespace SecurityManagement.Infrastructure.OutboxProcessor;

public class ProcessOutboxJob(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<ProcessOutboxJob> logger,
    IBus bus) : IJob
{
    private const int BatchSize = 30;

    public async Task Execute(IJobExecutionContext context)
    {
        await using var connection = await dbConnectionFactory.OpenConnectionAsync();

        if (connection is null)
        {
            return;
        }

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var pendingOutboxMessages = await connection.QueryAsync<OutboxMessage>(
                "SELECT * FROM (SELECT * FROM ACM.\"OutboxMessages\" WHERE \"Status\" = 1) WHERE ROWNUM <= :BatchSize FOR UPDATE SKIP LOCKED",
                new { BatchSize },
                transaction: transaction);

            var outboxMessages = pendingOutboxMessages.ToList();

            foreach (var message in outboxMessages)
            {
                try
                {
                    var payloadType = Type.GetType(message.PayloadType);

                    if (payloadType is null)
                    {
                        continue;
                    }

                    var payload = JsonSerializer.Deserialize(message.Payload, payloadType);

                    if (payload is not null)
                    {
                        await bus.Publish(payload, payloadType);
                    }

                    await connection.ExecuteAsync(
                        "DELETE FROM ACM.\"OutboxMessages\" WHERE \"Id\" = :Id",
                        new { message.Id },
                        transaction: transaction);
                }
                catch (Exception)
                {
                    // await connection.ExecuteAsync(
                    //     "UPDATE ACM.\"OutboxMessages\" SET \"Status\" = 2 WHERE \"Id\" = :Id",
                    //     new { message.Id },
                    //     transaction: transaction);
                }
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
        }
    }
}
