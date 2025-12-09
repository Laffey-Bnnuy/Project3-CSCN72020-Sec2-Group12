using System;
using System.Threading.Tasks;
using EV_SCADA.Modules;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== DOOR CONTROL SUBSYSTEM — SPRINT 2 DEMO ===\n");

        var door = new DoorControl();

        // Subscribe to state change events
        door.OnStateChanged += state =>
        {
            Console.WriteLine($"[EVENT] Door state changed → {state}");
        };

        //  Generate a token
        Console.WriteLine("\nGenerating 1-minute token...");
        var token = door.GenerateToken(TimeSpan.FromMinutes(1));
        Console.WriteLine($"Token Created: {token.Id}");

        //  Unlock using the token
        Console.WriteLine("\nUnlocking door...");
        await door.UnlockDoorAsync(token.Id);

        // Auto-relock demonstration
        Console.WriteLine("\nWaiting 6 seconds to show auto-relock...");
        await Task.Delay(6000);
        Console.WriteLine($"Door State After Auto-Relock: {door.State}");

        //  Show active tokens
        Console.WriteLine("\nActive Tokens:");
        foreach (var t in door.GetActiveTokens())
            Console.WriteLine($" - Token: {t.Id}");

        //  Revoke the token
        Console.WriteLine("\nRevoking token...");
        bool revoked = door.RevokeToken(token.Id);
        Console.WriteLine($"Revoke Result: {revoked}");

        //  Try unlocking with revoked token
        Console.WriteLine("\nAttempting unlock with revoked token...");
        bool unlockAttempt = await door.UnlockDoorAsync(token.Id);
        Console.WriteLine($"Unlock Attempt Success: {unlockAttempt}");
        Console.WriteLine($"Failed Attempts Count: {door.FailedAttempts}");

        //  Trigger fault state
        Console.WriteLine("\nTriggering fault state...");
        door.TriggerFault();
        Console.WriteLine($"Current State: {door.State}");

        //  Print system logs
        Console.WriteLine("\n=== ACCESS LOGS ===");
        door.PrintStatus();

        Console.WriteLine("\n=== END OF DEMO ===");
    }
}

