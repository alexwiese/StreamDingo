using StreamDingo;

Console.WriteLine("Hello from StreamDingo Examples!");

// Demonstrate the core event sourcing functionality
Console.WriteLine("Creating event store and basic components...");

var eventStore = new InMemoryEventStore();
var hashProvider = new BasicHashProvider();

Console.WriteLine("✓ In-memory event store created");
Console.WriteLine("✓ Basic hash provider created");
Console.WriteLine($"✓ StreamDingo core library successfully initialized");

// Simple demonstration
var testData = new { Message = "StreamDingo is working!", Version = "1.0.0" };
var hash = hashProvider.CalculateHash(testData);
Console.WriteLine($"✓ Hash calculation works: {hash[..16]}...");

Console.WriteLine("\nStreamDingo event sourcing library is ready for use!");
