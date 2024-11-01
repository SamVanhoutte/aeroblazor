using AeroBlazor.Configuration;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace AeroBlazor.Security;

public class TableStorageProviderTokenProvider : JwtTokenProvider, ITokenStorageProvider
{
    private TableStorageOptions tableStorageOptions;
    private TableClient tableClient;
    private string TableName => tableStorageOptions.AuthTableName ?? "aeroblazortokens";
    private string ClientName => tableStorageOptions.ClientName ?? "aeroblazor";

    public TableStorageProviderTokenProvider(IOptions<TableStorageOptions> tableStorageOptions)
    {
        if (tableStorageOptions?.Value?.StorageAccount == null)
            throw new NullReferenceException($"The Tablestorage options were not provided");
        this.tableStorageOptions = tableStorageOptions.Value;
    }

    public override async Task<IEnumerable<AuthToken>?> GetTokensAsync()
    {
        await EnsureTableClientAsync();
        var rows = await tableClient.QueryAsync<TableEntity>($"PartitionKey eq '{ClientName}'", maxPerPage: 1000)
            .ToListAsync().ConfigureAwait(false);
        return rows.Select(row => new AuthToken(row.RowKey, row["Value"].ToString() ?? ""));
    }

    public override async Task ClearTokensAsync()
    {
        await EnsureTableClientAsync();
        await tableClient.DeleteAsync();
    }

    public override async Task PersistTokensAsync(IEnumerable<AuthToken> tokens)
    {
        await EnsureTableClientAsync();
        var entities = tokens.Select(token => 
            new TableEntity(ClientName, token.Name)
            {
                { "Value", token.Value }
            });
        // Entity doesn't exist in table, so invoking UpsertEntity will simply insert the entity.
        foreach (var entity in entities)
        {
            await tableClient.UpsertEntityAsync(entity);
        }
    }
    

    private async Task<TableClient> EnsureTableClientAsync(bool createTableIfNotExists = true)
    {
        if (tableClient == null)
        {
            tableClient = new TableClient(
                tableStorageOptions.TableEndpointUri,
                TableName,
                new TableSharedKeyCredential(tableStorageOptions.StorageAccount,
                    tableStorageOptions.StorageAccountKey));
            if (createTableIfNotExists)
            {
                await tableClient.CreateIfNotExistsAsync();
            }
        }

        return tableClient;
    }
}