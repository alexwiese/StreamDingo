using StreamDingo;

// Example usage
Console.WriteLine("=== StreamDingo Event Sourcing Example ===\n");

var hashProvider = new Sha256HashProvider();
var accountOpenedHandler = new AccountOpenedEventHandler();
var moneyDepositedHandler = new MoneyDepositedEventHandler();

// Create initial empty snapshot
var initialSnapshot = new Snapshot<BankAccount>(
    "ACC-001", 
    0, 
    hashProvider.ComputeHash(new BankAccount()), 
    new BankAccount(),
    true
);

Console.WriteLine("Initial snapshot created");
Console.WriteLine($"Version: {initialSnapshot.Version}, Hash: {initialSnapshot.Hash[..8]}...\n");

// Open account event
var openEvent = new AccountOpenedEvent(
    accountOpenedHandler.CodeHash,
    "ACC-001",
    "John Doe",
    1000m
);

var afterOpenSnapshot = accountOpenedHandler.Apply(initialSnapshot, openEvent);
Console.WriteLine("Account opened event applied:");
Console.WriteLine($"Account ID: {afterOpenSnapshot.Data.AccountId}");
Console.WriteLine($"Owner: {afterOpenSnapshot.Data.Owner}");
Console.WriteLine($"Balance: ${afterOpenSnapshot.Data.Balance}");
Console.WriteLine($"Version: {afterOpenSnapshot.Version}, Hash: {afterOpenSnapshot.Hash[..8]}...\n");

// Deposit money event
var depositEvent = new MoneyDepositedEvent(moneyDepositedHandler.CodeHash, 500m);
var afterDepositSnapshot = moneyDepositedHandler.Apply(afterOpenSnapshot, depositEvent);

Console.WriteLine("Money deposited event applied:");
Console.WriteLine($"Account ID: {afterDepositSnapshot.Data.AccountId}");
Console.WriteLine($"Owner: {afterDepositSnapshot.Data.Owner}");
Console.WriteLine($"Balance: ${afterDepositSnapshot.Data.Balance}");
Console.WriteLine($"Version: {afterDepositSnapshot.Version}, Hash: {afterDepositSnapshot.Hash[..8]}...\n");

// Demonstrate hash integrity
Console.WriteLine("=== Hash Integrity Verification ===");
var recalculatedHash = hashProvider.ComputeHash(afterDepositSnapshot.Data);
var hashesMatch = recalculatedHash == afterDepositSnapshot.Hash;
Console.WriteLine($"Stored hash: {afterDepositSnapshot.Hash[..8]}...");
Console.WriteLine($"Recalculated hash: {recalculatedHash[..8]}...");
Console.WriteLine($"Hashes match: {hashesMatch}");

Console.WriteLine("\nExample completed successfully!");

// Define your entity
public record BankAccount
{
    public string AccountId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Owner { get; set; } = string.Empty;
}

// Define events
public class AccountOpenedEvent : EventBase<BankAccount>
{
    public string AccountId { get; }
    public string Owner { get; }
    public decimal InitialDeposit { get; }

    public AccountOpenedEvent(string handlerCodeHash, string accountId, string owner, decimal initialDeposit) 
        : base(handlerCodeHash)
    {
        AccountId = accountId;
        Owner = owner;
        InitialDeposit = initialDeposit;
    }
}

public class MoneyDepositedEvent : EventBase<BankAccount>
{
    public decimal Amount { get; }

    public MoneyDepositedEvent(string handlerCodeHash, decimal amount) : base(handlerCodeHash)
    {
        Amount = amount;
    }
}

// Define event handlers
public class AccountOpenedEventHandler : IEventHandler<BankAccount, AccountOpenedEvent>
{
    private readonly IHashProvider _hashProvider = new Sha256HashProvider();

    public string CodeHash => _hashProvider.ComputeCodeHash("AccountOpenedEventHandler_v1.0");

    public ISnapshot<BankAccount> Apply(ISnapshot<BankAccount> previousSnapshot, AccountOpenedEvent @event)
    {
        var account = new BankAccount
        {
            AccountId = @event.AccountId,
            Owner = @event.Owner,
            Balance = @event.InitialDeposit
        };

        return new Snapshot<BankAccount>(
            @event.AccountId,
            previousSnapshot.Version + 1,
            _hashProvider.ComputeHash(account),
            account
        );
    }
}

public class MoneyDepositedEventHandler : IEventHandler<BankAccount, MoneyDepositedEvent>
{
    private readonly IHashProvider _hashProvider = new Sha256HashProvider();

    public string CodeHash => _hashProvider.ComputeCodeHash("MoneyDepositedEventHandler_v1.0");

    public ISnapshot<BankAccount> Apply(ISnapshot<BankAccount> previousSnapshot, MoneyDepositedEvent @event)
    {
        var account = previousSnapshot.Data with
        {
            Balance = previousSnapshot.Data.Balance + @event.Amount
        };

        return new Snapshot<BankAccount>(
            previousSnapshot.EntityId,
            previousSnapshot.Version + 1,
            _hashProvider.ComputeHash(account),
            account
        );
    }
}
