using System;
using System.Threading;
using EVSystem.Communication;
using EVSystem.Components;
using EVSystem.Interfaces; // Added from dev_Daniel
using EVSystem.Mock;

namespace EVSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nEV SYSTEM DIAGNOSTICS SIMULATION STARTED\n");

            // --- J1939 Adapter ---
            // Create shared J1939 communication adapter
            var j1939 = new MockJ1939Adapter();
            var adapter = j1939; // Use 'j1939' consistently

            // --- Components from Master Branch ---
            var tireMonitor = new TirePressureMonitor(j1939);
            var camera = new MockRearViewCamera();
            var reverseAlerts = new ReverseAlerts(j1939, camera);
            var lightControl = new LightControl(j1939);
            
            // --- Components from dev_Daniel Branch ---
            IBattery battery = new BatteryMonitor(adapter);
            IChargingControl charger = new ChargingControl(adapter,battery);

            // ------------------------------------------------------------------
            // --- Master Branch Simulation Logic ---
            // ------------------------------------------------------------------

            // Enable automatic headlights
            lightControl.EnableAutoMode();

            Console.WriteLine("EV System Simulation");

            for (int i = 0; i < 5; i++) // Reduced from 10 to 5 for brevity/combining cycles
            {
                Console.WriteLine($"\n--- System Update Cycle #{i + 1} ---");

                // Update systems (Tire, Reverse Alerts, Lights)
                tireMonitor.UpdateTirePressures();
                bool tireStatus = tireMonitor.CheckTirePressure();
                Console.WriteLine(tireMonitor.GetStatus());

                reverseAlerts.UpdateSensorData();
                reverseAlerts.DetectObstacle();
                Console.WriteLine(reverseAlerts.GetStatus());

                lightControl.UpdateAmbientLight();
                Console.WriteLine(lightControl.GetStatus());

                // --- Incorporate dev_Daniel Logic into the loop ---
                
                // Battery status update
                battery.UpdateBatteryLevel(95 - (i * 5)); // Simulate dropping battery level
                Console.WriteLine($"\nBattery current status (Cycle {i+1}):");
                Console.WriteLine(battery.GetStatus());

                Console.WriteLine("\n--- Waiting 2 seconds before next cycle ---\n");
                Thread.Sleep(2000);
            }

            // ------------------------------------------------------------------
            // --- End-of-Simulation Actions (can be outside the loop) ---
            // ------------------------------------------------------------------

            Console.WriteLine("\n=== Simulation Complete. Shutting down systems. ===\n");

            // Final actions from dev_Daniel
            Console.WriteLine("\nSet battery mode to sport");
            battery.SetBatteryMode("Sport");

            Console.WriteLine("\nSchedule charging for tonight");
            charger.ScheduleCharging("23:00 - 05:00");
            Console.WriteLine($"The schedule is: {charger.Schedule}");
            
            // Cleanup / shutdown actions (from both branches)
            camera.DeactivateCamera();
            adapter.Leave(); // Use adapter.Leave() or j1939.Leave()

            Console.WriteLine("\n=== EV SYSTEM DIAGNOSTICS ENDED ===");
        }
    }
}