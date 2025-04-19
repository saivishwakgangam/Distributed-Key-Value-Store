using ReplicatedKeyValueStore.Interfaces;
using ReplicatedKeyValueStore.Models;

namespace ReplicatedKeyValueStore
{
    public class FollowerNode : INode
    {
        public string NodeId { get; }
        private readonly KeyValueStore _store;
        private long _lastAppliedOpTimestamp = 0;
        private bool _isOnline = true;
        
        public FollowerNode(string nodeId)
        {
            NodeId = nodeId;
            _store = KeyValueStore.GetInstance(NodeId);
        }
        
        public string Get(string key)
        {
            if (!_isOnline)
            {
                throw new InvalidOperationException($"Follower {NodeId} is offline");
            }
            return _store.Get(key);
        }
        
        // Follower doesn't accept direct writes
        public void Put(string key, string value)
        {
            throw new InvalidOperationException($"Cannot write directly to follower node {NodeId}");
        }
        
        public void Delete(string key)
        {
            throw new InvalidOperationException($"Cannot delete directly from follower node {NodeId}");
        }
        
        public void Replicate(Operation op)
        {
            if (!_isOnline)
            {
                Console.WriteLine($"Follower {NodeId}: Cannot replicate while offline");
                return;
            }
            
            // Simulate network delay
            Task.Delay(100).Wait();
            
            if (op.Timestamp > _lastAppliedOpTimestamp)
            {
                if (op.Type == "PUT")
                {
                    _store.Put(op.Key, op.Value);
                    Console.WriteLine($"Follower {NodeId}: Applied PUT {op.Key}={op.Value}");
                }
                else if (op.Type == "DELETE")
                {
                    if (_store.Delete(op.Key))
                    {
                        Console.WriteLine($"Follower {NodeId}: Applied DELETE {op.Key}");
                    }
                }
                
                _lastAppliedOpTimestamp = op.Timestamp;
            }
        }
        
        public void GoOffline()
        {
            _isOnline = false;
            Console.WriteLine($"Follower {NodeId}: Going offline");
        }
        
        public void GoOnline()
        {
            _isOnline = true;
            Console.WriteLine($"Follower {NodeId}: Coming back online");
        }
        
        public bool IsOnline()
        {
            return _isOnline;
        }
        
        public long GetReplicationLag()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _lastAppliedOpTimestamp;
        }
    }
}
