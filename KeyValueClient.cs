using System;
using System.Collections.Generic;

namespace ReplicatedKeyValueStore
{
    public class KeyValueClient
    {
        private LeaderNode _leader;
        private List<FollowerNode> _readPool = new List<FollowerNode>();
        
        public KeyValueClient(LeaderNode leader)
        {
            _leader = leader;
        }
        
        public void AddNodeToReadPool(FollowerNode node)
        {
            _readPool.Add(node);
        }
        
        public void Put(string key, string value)
        {
            _leader.Put(key, value);
        }
        
        public string Get(string key, bool readFromLeader = false)
        {
            // Can read from leader or any follower
            if (readFromLeader || _readPool.Count == 0)
            {
                return _leader.Get(key);
            }
            else
            {
                // Find an online follower
                var onlineFollowers = _readPool.FindAll(f => f.IsOnline());
                if (onlineFollowers.Count == 0)
                {
                    Console.WriteLine("No online followers available, reading from leader");
                    return _leader.Get(key);
                }
                
                // Simple round robin for demonstration
                var randomIndex = new Random().Next(onlineFollowers.Count);
                var selectedNode = onlineFollowers[randomIndex];
                Console.WriteLine($"Reading from follower: {selectedNode.NodeId}");
                
                try
                {
                    return selectedNode.Get(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading from follower: {ex.Message}");
                    return _leader.Get(key);
                }
            }
        }
        
        public void Delete(string key)
        {
            _leader.Delete(key);
        }
    }
}