using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace BackOffice.PlayerSearch.DynamoDb;

public class PlayerSearchService(IAmazonDynamoDB dynamoDb)
{
    public async Task<List<PlayerDto>> FindPlayersAsync(string input, int maxResults = 10, CancellationToken ct = default)
    {
        // Use a larger scan limit to increase chance of getting enough matches
        var scanLimit = Math.Max(maxResults * 10, 100);

        var request = new ScanRequest
        {
            TableName = PlayerDto.TableName,
            FilterExpression = "contains(UsernameText, :input)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":input", new AttributeValue { S = input } }
            },
            Limit = scanLimit
        };

        var response = await dynamoDb.ScanAsync(request, ct);

        var results = response.Items
            .Select(item => new PlayerDto
            {
                Id = long.Parse(item["Id"].S),
                Username = item["UsernameText"].S
            })
            .Take(maxResults)
            .ToList();

        return results;
    }
    
    public Task AddOrUpdatePlayerAsync(long playerId, string username, CancellationToken ct)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["Id"] = new() { S = playerId.ToString() },
            ["UsernameText"] = new() { S = username },
        };

        var request = new PutItemRequest
        {
            TableName = PlayerDto.TableName,
            Item = item
        };

        return dynamoDb.PutItemAsync(request, ct);
    }
}