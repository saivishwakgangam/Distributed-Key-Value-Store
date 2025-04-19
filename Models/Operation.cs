namespace ReplicatedKeyValueStore.Models
{
    /// <summary>
    /// Operation class represents a single operation in the replicated key-value store.
    /// </summary>
    public class Operation
    {
        public string Type { get; set; }  // "PUT" or "DELETE"
        public string Key { get; set; }
        public string Value { get; set; } // null for DELETE operations
        public long Timestamp { get; set; }
        public string OriginNodeId { get; set; }

        public Operation(string type, string key, string value, long timestamp, string originNodeId)
        {
            Type = type;
            Key = key;
            Value = value;
            Timestamp = timestamp;
            OriginNodeId = originNodeId;
        }
    }
}