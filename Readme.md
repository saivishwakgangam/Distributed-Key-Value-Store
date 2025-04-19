Simple Replicated InMemory key-value store in c# that demonstrates fundamental distributed system concepts focussing on leader - follower replication.


## Features
- Simple key-value store
- Basic operations: (Put, Get and Delete)
- Leader-Follower replication model where there is single leader and multiple followers.
- Read your writes consistency
- Basic Fault Tolerance
- Simple Replication Monitoring and replication happens in sync manner rather than async.

```mermaid
graph TD
Client[Client] --> Leader[LeaderNode]
Leader --> Follower1[FollowerNode 1]
Leader --> Follower2[FollowerNode 2]
Follower1 --> KVStore1[KeyValueStore 1]
Follower2 --> KVStore2[KeyValueStore 2]
```






