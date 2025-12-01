using System;
using System.Threading;
using EVSystem.Communication;
using EVSystem.Components;
using EVSystem.Mock;

namespace EVSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nEV SYSTEM DIAGNOSTICS SIMULATION STARTED\n");

            // Create shared J1939 communication adapter
            J1939Adapter j1939 = new MockJ1939Adapter();

            // Create components
            var tireMonitor = new TirePressureMonitor(j1939);
            var camera = new MockRearViewCamera();
            var reverseAlerts = new ReverseAlerts(j1939, camera);
            var lightControl = new LightControl(j1939);

            // Enable automatic headlights
            lightControl.EnableAutoMode();

            for (int i = 0; i < 10; i++) // Simulate 10 cycles
            {
                Console.WriteLine($"\n--- System Update Cycle #{i + 1} ---");

                // Update systems
                tireMonitor.UpdateTirePressures();
                bool tireStatus = tireMonitor.CheckTirePressure();
                Console.WriteLine(tireMonitor.GetStatus());

                reverseAlerts.UpdateSensorData();
                reverseAlerts.DetectObstacle();
                Console.WriteLine(reverseAlerts.GetStatus());

                lightControl.UpdateAmbientLight();
                Console.WriteLine(lightControl.GetStatus());

                Console.WriteLine("\n--- Waiting 2 seconds before next cycle ---\n");
                Thread.Sleep(2000);
            }

            Console.WriteLine("\n=== Simulation Complete. Shutting down systems. ===\n");

            // Cleanup / shutdown actions
            camera.DeactivateCamera();

            Console.WriteLine("\n=== EV SYSTEM DIAGNOSTICS ENDED ===");
        }
    }
}
