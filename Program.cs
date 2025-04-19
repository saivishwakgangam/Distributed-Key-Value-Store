using ReplicatedKeyValueStore.Interfaces;

namespace ReplicatedKeyValueStore;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting Replicated Key-Value Store Demo");
            
        // Create leader and followers
        var leader = new LeaderNode();
        var follower1 = new FollowerNode("follower1");
        var follower2 = new FollowerNode("follower2");
            
        // Register followers with leader
        leader.RegisterFollower(follower1);
        leader.RegisterFollower(follower2);
            
        // Create client
        var client = new KeyValueClient(leader);
        client.AddNodeToReadPool(follower1);
        client.AddNodeToReadPool(follower2);
        
        // Basic operation test
        RunBasicOperationsTest(client);
            
        // Consistency test
        RunConsistencyTest(client);
            
        // Delete operation test
        RunDeleteTest(client);
            
        // Follower failure test
        RunFollowerFailureTest(client, follower1, leader);
            
        // Interactive Mode
        RunInteractiveMode(client, leader, follower1, follower2);
        
    }
    
    static void RunBasicOperationsTest(KeyValueClient client)
    {
        Console.WriteLine("\n=== Basic Operations Test ===");
        client.Put("name", "John");
            
        // Give time for replication
        Thread.Sleep(200);
            
        Console.WriteLine($"Reading from leader: {client.Get("name", true)}");
        Console.WriteLine($"Reading from random follower: {client.Get("name")}");
    }
    
    static void RunConsistencyTest(KeyValueClient client)
    {
        Console.WriteLine("\n=== Consistency Test ===");
        client.Put("counter", "1");
            
        // Immediate read should return latest value due to "Read Your Writes" consistency
        Console.WriteLine($"Immediate read after write: {client.Get("counter", true)}");
            
        // Update value
        client.Put("counter", "2");
        Thread.Sleep(200);
        Console.WriteLine($"After replication: {client.Get("counter")}");
    }
    
    static void RunDeleteTest(KeyValueClient client)
    {
        Console.WriteLine("\n=== Delete Operation Test ===");
        client.Put("temporary", "value");
        Thread.Sleep(200);
        Console.WriteLine($"Before delete: {client.Get("temporary")}");
            
        client.Delete("temporary");
        Thread.Sleep(200);
        Console.WriteLine($"After delete: {client.Get("temporary")}");
    }
    
    static void RunFollowerFailureTest(KeyValueClient client, FollowerNode follower, LeaderNode leader)
    {
        Console.WriteLine("\n=== Follower Failure and Recovery Test ===");
            
        // 1. Put a value with all nodes online
        client.Put("before-failure", "online-value");
        Thread.Sleep(200);
            
        // 2. Take a follower offline
        follower.GoOffline();
            
        // 3. Put a new value with follower offline
        client.Put("during-failure", "offline-value");
        Thread.Sleep(200);
            
        // 4. Try to read from this follower (should fail over to leader)
        Console.WriteLine($"Reading during follower offline: {client.Get("during-failure")}");
            
        // 5. Bring follower back online
        follower.GoOnline();
            
        // 6. Simulate catch-up (in a real implementation, this would be more sophisticated)
        // Re-register to trigger full sync
        leader.UnregisterFollower(follower.NodeId);
        leader.RegisterFollower(follower);
            
        Thread.Sleep(300);
            
        // 7. Verify follower has caught up
        Console.WriteLine($"Reading from recovered follower: {client.Get("during-failure")}");
    }

    static void RunInteractiveMode(KeyValueClient client, LeaderNode leader,
        FollowerNode follower1, FollowerNode follower2)
    {
        Console.WriteLine("\n=== Interactive Mode ===");
        Console.WriteLine(
            "Enter commands (PUT key value, GET key, DELETE key, OFFLINE follower#, ONLINE follower#, EXIT):");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ');
            var command = parts[0].ToUpper();

            try
            {
                switch (command)
                {
                    case "PUT":
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: PUT key value");
                            break;
                        }

                        client.Put(parts[1], string.Join(" ", parts, 2, parts.Length - 2));
                        break;

                    case "GET":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: GET key");
                            break;
                        }

                        Console.WriteLine(client.Get(parts[1]));
                        break;

                    case "DELETE":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: DELETE key");
                            break;
                        }

                        client.Delete(parts[1]);
                        break;

                    case "OFFLINE":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: OFFLINE follower1|follower2");
                            break;
                        }

                        if (parts[1] == "follower1")
                            follower1.GoOffline();
                        else if (parts[1] == "follower2")
                            follower2.GoOffline();
                        else
                            Console.WriteLine("Unknown follower. Use follower1 or follower2");
                        break;

                    case "ONLINE":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: ONLINE follower1|follower2");
                            break;
                        }

                        if (parts[1] == "follower1")
                            follower1.GoOnline();
                        else if (parts[1] == "follower2")
                            follower2.GoOnline();
                        else
                            Console.WriteLine("Unknown follower. Use follower1 or follower2");
                        break;

                    case "EXIT":
                        return;

                    default:
                        Console.WriteLine(
                            "Unknown command. Available commands: PUT, GET, DELETE, OFFLINE, ONLINE, EXIT");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
