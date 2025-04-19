namespace ReplicatedKeyValueStore.Interfaces
{
    /// <summary>
    /// INode interface represents a node in the replicated key-value store.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Gets the ID of the node.
        /// </summary>
        string NodeId { get; }
        
        /// <summary>
        /// Get the value associated with the given key.
        /// </summary>
        string Get(string key);
        
        /// <summary>
        /// Put a key-value pair into the store.
        /// </summary>
        void Put(string key, string value);
        
        /// <summary>
        /// Delete the key from the store.
        /// </summary>
        void Delete(string key);
    }
}

