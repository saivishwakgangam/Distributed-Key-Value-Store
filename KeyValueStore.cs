using ReplicatedKeyValueStore.Interfaces;

namespace ReplicatedKeyValueStore;
using System.Collections.Concurrent;

public class KeyValueStore
{
    // Singleton pattern - dictionary to store instances per node ID
    private static readonly ConcurrentDictionary<string, KeyValueStore> _instances 
        = new ConcurrentDictionary<string, KeyValueStore>();
        
    // The actual data store
    private ConcurrentDictionary<string, string> _dataStore = new ConcurrentDictionary<string, string>();
        
    // Private constructor to prevent direct instantiation
    private KeyValueStore() { }
        
    // Get the singleton instance for a specific node
    public static KeyValueStore GetInstance(string nodeId)
    {
        return _instances.GetOrAdd(nodeId, _ => new KeyValueStore());
    }
        
    // Basic operations
    public string Get(string key)
    {
        return _dataStore.TryGetValue(key, out string value) ? value : null;
    }
        
    public bool Put(string key, string value)
    {
        _dataStore[key] = value;
        return true;
    }
        
    public bool Delete(string key)
    {
        return _dataStore.TryRemove(key, out _);
    }
        
    public void Clear()
    {
        _dataStore.Clear();
    }
}

