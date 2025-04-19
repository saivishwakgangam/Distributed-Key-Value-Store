// Define a factory interface for creating follower nodes
 
 using ReplicatedKeyValueStore.Interfaces;
 
 public interface IFollowerNodeFactory
 {
     /// <summary>
     /// Creates a follower node.
     /// </summary>
     /// <param name="nodeId">The ID of the node.</param>
     /// <returns>A new follower node.</returns>
     INode CreateFollowerNode(string nodeId);
 }