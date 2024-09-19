using Dapper;
using MassTransit;
using MtslErp.Common.Application.Data;
using MtslErp.Common.Domain.Events;

namespace PrintFactoryManagement.Application.Consumers;

public sealed class UserRegisteredEventConsumer(
    IDbConnectionFactory dbConnectionFactory)
    : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        Console.WriteLine($"User Registered Event Received: {context.Message.UserName}");

        await using var connection = await dbConnectionFactory.OpenConnectionAsync();

        if (connection is null)
        {
            return;
        }

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var result = await connection.QuerySingleOrDefaultAsync<string?>(
                "SELECT \"Id\" FROM PFM.\"InboxStates\" WHERE \"MessageId\" = :MessageId",
                new { MessageId = context.MessageId.ToString() },
                transaction: transaction);

            if (result is not null)
            {
                return;
            }

            await connection.ExecuteAsync(
                "INSERT INTO PFM.\"InboxStates\" (\"MessageId\", \"ProcessedOn\") VALUES (:MessageId, :ProcessedOn)",
                new { MessageId = context.MessageId.ToString(), ProcessedOn = DateTime.UtcNow },
                transaction: transaction);

            await connection.ExecuteAsync(
                "INSERT INTO PFM.\"Users\" " +
                "(\"FullName\", \"UserName\", \"Email\", \"PhoneNumber\", \"DateOfBirth\", \"Address\", \"ProfilePictureUri\") " +
                "VALUES (:FullName, :UserName, :Email, :PhoneNumber, :DateOfBirth, :Address, :ProfilePictureUri)",
                new
                {
                    FullName = context.Message.FullName,
                    UserName = context.Message.UserName,
                    Email = context.Message.Email,
                    PhoneNumber = context.Message.PhoneNumber,
                    DateOfBirth = context.Message.DateOfBirth.ToString(),
                    Address = context.Message.Address,
                    ProfilePictureUri = context.Message.ProfilePictureUri
                },
                transaction: transaction);

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
        }
    }
}
