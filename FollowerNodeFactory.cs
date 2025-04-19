using ReplicatedKeyValueStore.Interfaces;

namespace ReplicatedKeyValueStore;

public class FollowerNodeFactory(KeyValueStore keyValueStore) : IFollowerNodeFactory
{
    /// <summary>
    /// Creates a follower node.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <returns>A new follower node.</returns>
    public INode CreateFollowerNode(string nodeId)
    {
        return new FollowerNode(keyValueStore, nodeId);
    }
}