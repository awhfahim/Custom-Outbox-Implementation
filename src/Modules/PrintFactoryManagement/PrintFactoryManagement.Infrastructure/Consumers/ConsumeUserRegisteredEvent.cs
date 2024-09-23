using Dapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using MtslErp.Common.Application.Data;
using MtslErp.Common.Domain.Events;

namespace PrintFactoryManagement.Infrastructure.Consumers;

public sealed class UserRegisteredEventConsumer(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<UserRegisteredEventConsumer> logger)
{
    // Kept for reference
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        logger.LogInformation("Processing UserRegisteredEvent.");

        await using var connection = await dbConnectionFactory.OpenConnectionAsync();

        if (connection is null)
        {
            return;
        }

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var result = await connection.QuerySingleOrDefaultAsync<string?>(
                "SELECT \"Id\" FROM PFM.INBOX_STATES WHERE \"MessageId\" = :MessageId",
                new { MessageId = context.MessageId.ToString() },
                transaction: transaction);

            if (result is not null)
            {
                await context.ConsumeCompleted;
                return;
            }

            await connection.ExecuteAsync(
                "INSERT INTO PFM.INBOX_STATES (\"MessageId\", \"ProcessedOn\") VALUES (:MessageId, :ProcessedOn)",
                new { MessageId = context.MessageId.ToString(), ProcessedOn = DateTime.UtcNow },
                transaction: transaction);

            await connection.ExecuteAsync(
                "INSERT INTO PFM.USERS " +
                "(\"FullName\", \"UserName\", \"Email\", \"PhoneNumber\", \"DateOfBirth\", \"Address\", \"ProfilePictureUri\") " +
                "VALUES (:FullName, :UserName, :Email, :PhoneNumber, :DateOfBirth, :Address, :ProfilePictureUri)",
                new
                {
                    FullName = context.Message.FullName,
                    UserName = context.Message.UserName,
                    Email = context.Message.Email,
                    PhoneNumber = context.Message.PhoneNumber,
                    DateOfBirth = context.Message.DateOfBirth,
                    Address = context.Message.Address,
                    ProfilePictureUri = context.Message.ProfilePictureUri
                },
                transaction: transaction);

            await transaction.CommitAsync();
            logger.LogInformation("User Registered Event Processed Successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error processing UserRegisteredEvent. Rolling back transaction and scheduling redelivery.");
            await transaction.RollbackAsync();
            await context.Redeliver(TimeSpan.FromMinutes(2));
        }
    }
}
