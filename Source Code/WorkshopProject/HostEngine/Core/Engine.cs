using Azure;
using Azure.AI.OpenAI;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Azure.Search.Documents;
using System.Text;

//reference: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
//reference: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/redis-cache?source=recommendations&view=azure-dotnet
//reference: https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/quickstart-dotnet?tabs=azure-portal%2Cwindows%2Cconnection-string%2Csign-in-azure-cli

namespace HostEngine.Core
{
    public class EngineConfig
    {
        public string AzureOpenAIAPIURL { get; set; }
        public string AzureOpenAIKey { get; set; }
        public string AzureOpenAIDeploymentId { get; set; }
        public float AzureOpenAITemperature { get; set; }
        public string RedisConnectionString { get; set; }
        public string CosmosDBConnectionString { get; set; }
        public string CosmosDBName { get; set; }
        public string EnterpriseBotName { get; set; }
        public string EnterpriseName { get; set; }

        public string EnterpriseNoAnswerFallback { get; set; }
        public string EnterpriseSearchApiURL { get; set; }
        public string EntepriseSearchApiKey { get; set; }
        public string EnterpriseSearchIndex { get; set; }
    }

    public class CompletionCacheItem
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string prompt { get; set; }
        public string completion { get; set; }
    }

    public class EnterpriseDataItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class EnterpriseDataSearchResult
    {
        public long Total { get; set; }
        public List<EnterpriseDataItem> Items { get; set; }
    }

    public interface IEngine
    {
        Task<string> GetCompletionAsync(string userId, string prompt);
    }
    public class Engine: IEngine
    {
        private readonly EngineConfig _config;
        private readonly OpenAIClient _openAIClient;
        private static CosmosClient _cosmosDbClient;
        private static SearchClient _searchClient;
        const int MaxTokens = 2048;
        public Engine(IOptions<EngineConfig> config)
        {
            _config = config.Value;
            _openAIClient = new OpenAIClient(new Uri(_config.AzureOpenAIAPIURL), new AzureKeyCredential(_config.AzureOpenAIKey));
        }
        public async Task<string> GetCompletionAsync(string userId, string prompt)
        {
            var cachedCompletion = await TryGetCachedCompletionAsync(prompt);
            if (cachedCompletion != null)
            {
                return cachedCompletion;
            }
            else
            {
                var internalData = await SearchEnterpriseDataAsync(prompt);
                var internalResult = internalData?.Content ?? "";
                var options = new CompletionsOptions
                {
                    User = userId,
                    MaxTokens = MaxTokens,
                    Prompt = { BuildPromptGPT3(prompt, internalResult) }
                };
                var completions = await _openAIClient.GetCompletionsAsync(_config.AzureOpenAIDeploymentId, options);
                var completion = completions.Value.Choices[0].Text;

                await AddToCacheAsync(prompt, completion);
                await SaveToDatabaseAsync(userId, prompt, completion);

                return completion;
            }
        }
        private async Task<string> TryGetCachedCompletionAsync(string prompt)
        {
            if (!string.IsNullOrEmpty(_config.RedisConnectionString))
            {
                var redisConnection = ConnectionMultiplexer.Connect(_config.RedisConnectionString);
                IDatabase cache = redisConnection.GetDatabase();
                var cachedValue = await cache.StringGetAsync(prompt);
                if (!cachedValue.IsNull)
                {
                    return (string)cachedValue;
                }
            }
            return null;
        }
        private async Task AddToCacheAsync(string prompt, string completion)
        {
            //add to cache
            if(string.IsNullOrEmpty(_config.RedisConnectionString))
            {
                return;
            }
            var redisConnection = ConnectionMultiplexer.Connect(_config.RedisConnectionString);
            IDatabase cache = redisConnection.GetDatabase();
            await cache.StringSetAsync(prompt, completion);
        }
        private async Task SaveToDatabaseAsync(string userId, string prompt, string completion)
        {
            if (string.IsNullOrEmpty(_config.CosmosDBConnectionString))
            {
                return;
            }

            if (_cosmosDbClient == null)
            {
                _cosmosDbClient = new CosmosClient(_config.CosmosDBConnectionString);
            }

            var database = _cosmosDbClient.GetDatabase(_config.CosmosDBName);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                            id: "completions",
                            partitionKeyPath: "/userId",
                            throughput: 400
            );

            await containerResponse.Container.CreateItemAsync(new CompletionCacheItem
            {
                id = Guid.NewGuid().ToString(),
                userId = userId,
                prompt = prompt,
                completion = completion
            });
        }
        private async Task<string> GetFromDatabaseAsync(string prompt)
        {
            if (string.IsNullOrEmpty(_config.CosmosDBConnectionString))
            {
                return null;
            }

            var database = _cosmosDbClient.GetDatabase(_config.CosmosDBName);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                            id: "completions",
                            partitionKeyPath: "/userId",
                            throughput: 400
            );
            var query = new QueryDefinition(query: "SELECT * FROM completions p WHERE p.prompt = @prompt").WithParameter("@prompt", prompt);

            var feed = containerResponse.Container.GetItemQueryIterator<CompletionCacheItem>(query);

            while (feed.HasMoreResults)
            {
                FeedResponse<CompletionCacheItem> response = await feed.ReadNextAsync();
                foreach (CompletionCacheItem item in response)
                {
                    return item.completion;
                }
            }
            return null;
        }
        private async Task<EnterpriseDataItem> SearchEnterpriseDataAsync(string prompt)
        {
            if(string.IsNullOrEmpty(_config.EnterpriseSearchApiURL))
            {
                return null;
            }

            if(_searchClient == null)
            {
                _searchClient = new SearchClient(new Uri(_config.EnterpriseSearchApiURL), _config.EnterpriseSearchIndex, new AzureKeyCredential(_config.EntepriseSearchApiKey));
            }
           
            var searchResponse = await _searchClient.SearchAsync<EnterpriseDataItem>(prompt);
            var result = new EnterpriseDataSearchResult
            {
                Total = searchResponse.Value.TotalCount ?? 0,
                Items = new List<EnterpriseDataItem>()
            };
            await foreach (var page in searchResponse.Value.GetResultsAsync().AsPages())
            {
                foreach (var item in page.Values)
                {
                    result.Items.Add(new EnterpriseDataItem
                    {
                        Id = item.Document.Id,
                        Name = item.Document.Name,
                        Content = item.Document.Content
                    });
                }
                break;
            }
            return result.Items.FirstOrDefault();
        }
        private string BuildPromptGPT3(string prompt, string internalFacts)
        {
            StringBuilder prompBuilder = new StringBuilder();
            if(!string.IsNullOrEmpty(_config.EnterpriseSearchApiURL))
            {
                prompBuilder.Append($"{_config.EnterpriseBotName}是一个智能助理为{_config.EnterpriseName}员工提供知识问答服务。");
                prompBuilder.Append($"\n\n 请根据如下列出的事实回答提问，如果列出的事实不能回答提问，请回答\"{_config.EnterpriseNoAnswerFallback}\"");
                prompBuilder.Append($"\n\n{(internalFacts.Length > 3000 ? internalFacts.Substring(0, 3000) : internalFacts)}");
            }            
            prompBuilder.Append($"\n\nQ:{prompt}");
            prompBuilder.Append("\nA:");
            return prompBuilder.ToString();
        }
    }
}
