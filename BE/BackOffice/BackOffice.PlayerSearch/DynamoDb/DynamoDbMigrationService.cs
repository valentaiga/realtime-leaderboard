using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace BackOffice.PlayerSearch.DynamoDb;

public class DynamoDbMigrationService(IAmazonDynamoDB dynamoDb, ILogger<DynamoDbMigrationService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken ct) =>
        MigrateAsync(ct);

    private async Task MigrateAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
            try
            {
                logger.LogInformation("Applying migrations...");
                var tables = await dynamoDb.ListTablesAsync(ct);
                var migrationsApplied = false;

                if (!tables.TableNames.Contains(PlayerDto.TableName))
                {
                    await CreatePlayersTableAsync(ct);
                    migrationsApplied = true;
                }

                if (migrationsApplied)
                    await Task.Delay(5_000, ct);

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (migrationsApplied)
                    logger.LogInformation("Migration applied successfully");
                else
                    logger.LogInformation("Database is up to date, no migrations applied");
                break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during migration apply");
                await Task.Delay(5_000, ct);
            }
    }

    private async Task CreatePlayersTableAsync(CancellationToken ct)
    {
        logger.LogDebug($"Applying migration {nameof(CreatePlayersTableAsync)}");
        var request = new CreateTableRequest
        {
            TableName = PlayerDto.TableName,
            AttributeDefinitions =
            [
                new AttributeDefinition("Id", ScalarAttributeType.S)
            ],
            KeySchema =
            [
                new KeySchemaElement("Id", KeyType.HASH)
            ],
            BillingMode = BillingMode.PAY_PER_REQUEST
        };

        await dynamoDb.CreateTableAsync(request, ct);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}