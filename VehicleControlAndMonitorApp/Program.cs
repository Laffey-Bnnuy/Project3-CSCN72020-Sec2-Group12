using System;
using System.Threading;
using EVSystem.Communication;
using EVSystem.Components;
using EVSystem.Interfaces;
using EVSystem.Mock;

namespace EVSystem.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== EV System Test ===\n");

            // Create mock adapter
            J1939Adapter adapter = new MockJ1939Adapter();

            // Create battery system
            IBattery battery = new BatteryMonitor(adapter);

            // Create charging controller
            ChargingControl charger = new ChargingControl(adapter, battery);

           
            Console.WriteLine("\nSystem running... Press Ctrl+C to exit.\n");

            // Simulation loop
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"--- Cycle {i + 1} ---");

                // Update battery data from CSV list
                battery.LoadNextData();

                // Execute charging cycle (charge increases only if IsCharging == true)
                charger.LoadNextData();

                // Print system status
                Console.WriteLine(battery.GetStatus());
                Console.WriteLine($"Charging set {(charger.IsCharging ? "ACTIVE" : "OFF")} | Limit: {charger.ChargeLimit}% | Schedule: {charger.Schedule}");
                Console.WriteLine();

                Thread.Sleep(1000); // Just for readable output in console
            }

            Console.WriteLine("\n=== Test Complete ===");
        }
    }
}
