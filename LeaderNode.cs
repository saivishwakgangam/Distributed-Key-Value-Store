using ReplicatedKeyValueStore;
using ReplicatedKeyValueStore.Interfaces;
using ReplicatedKeyValueStore.Models;

public class LeaderNode : INode
{
        public string NodeId { get; }
        private readonly KeyValueStore _store;
        private List<Operation> _replicationLog = new List<Operation>();
        private List<FollowerNode> _followers = new List<FollowerNode>();
        
        public LeaderNode(string nodeId = "leader")
        {
            NodeId = nodeId;
            _store = KeyValueStore.GetInstance(NodeId);
        }
        
        public string Get(string key)
        {
            return _store.Get(key);
        }
        
        public void Put(string key, string value)
        {
            // Update local store
            _store.Put(key, value);
            
            // Create operation for replication
            var op = new Operation("PUT", key, value, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), NodeId);
            _replicationLog.Add(op);
            
            // Replicate to followers
            foreach (var follower in _followers)
            {
                follower.Replicate(op);
            }

            Console.WriteLine($"Leader {NodeId}: PUT {key}={value}");
        }
        
        public void Delete(string key)
        {
            if (_store.Delete(key))
            {
                var op = new Operation("DELETE", key, null, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), NodeId);
                _replicationLog.Add(op);
                
                foreach (var follower in _followers)
                {
                    follower.Replicate(op);
                }

                Console.WriteLine($"Leader {NodeId}: DELETE {key}");
            }
        }
        
        public void RegisterFollower(FollowerNode follower)
        {
            _followers.Add(follower);
            Console.WriteLine($"Leader {NodeId}: Registered follower {follower.NodeId}. Total: {_followers.Count}");
            
            // Sync new follower with existing data
            foreach (var op in _replicationLog)
            {
                follower.Replicate(op);
            }
        }
        
        public bool UnregisterFollower(string followerId)
        {
            var follower = _followers.Find(f => f.NodeId == followerId);
            if (follower != null)
            {
                _followers.Remove(follower);
                Console.WriteLine($"Leader {NodeId}: Unregistered follower {followerId}. Remaining: {_followers.Count}");
                return true;
            }
            return false;
        }
        
        public IReadOnlyList<FollowerNode> GetFollowers()
        {
            return _followers.AsReadOnly();
        }
}