using Azure;
using Azure.Data.Tables;
using System;
using System.Text.Json.Serialization;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal abstract class AbstractEntityClass : ITableEntity
    {
        [JsonIgnore]
        public string RowKey { get; set; }
        [JsonIgnore]
        public string PartitionKey { get; set; } = "default";
        [JsonIgnore]
        public ETag ETag { get; set; } = default!;
        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
