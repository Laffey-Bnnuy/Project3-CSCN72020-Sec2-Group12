using System;
using EVSystem.Interfaces;
using EVSystem.Components;
using EVSystem.Mock;
using EVSystem.Communication;

class Program
{
    static void Main(string[] args)
    {
        var adapter = new MockJ1939Adapter();

        // Member 1 Components
        IBattery battery = new BatteryMonitor(adapter);
        IChargingControl charger = new ChargingControl(adapter);

        // Member 4 Components
        IRearViewCamera camera = new MockRearViewCamera();
        IReverseAlerts reverseAlerts = new ReverseAlerts(adapter, camera);
        ILightControl lightControl = new LightControl(adapter);
        ITirePressureMonitor tireMonitor = new TirePressureMonitor(adapter);

        Console.WriteLine("==================================================");
        Console.WriteLine("    EV SYSTEM SIMULATION - ALL COMPONENTS");
        Console.WriteLine("==================================================\n");

        // === MEMBER 1 DEMO ===
        Console.WriteLine("--- MEMBER 1: Battery & Charging ---");
        Console.WriteLine(battery.GetStatus());
        battery.SetBatteryMode("Sport");
        Console.WriteLine(battery.GetStatus());
        battery.UpdateBatteryLevel(80);
        Console.WriteLine(battery.GetStatus());
        charger.ScheduleCharging("23:00 - 05:00");
        Console.WriteLine($"Charging Schedule: {charger.GetSchedule()}\n");

        // === MEMBER 4 DEMO ===
        Console.WriteLine("\n--- MEMBER 4: Safety & Lighting Systems ---\n");

        // 1. Tire Pressure Monitoring
        Console.WriteLine("=== TIRE PRESSURE MONITOR ===");
        Console.WriteLine(tireMonitor.GetStatus());
        tireMonitor.CheckTirePressure();
        Console.WriteLine("\nUpdating tire pressures...");
        tireMonitor.UpdateTirePressures();
        Console.WriteLine(tireMonitor.GetStatus());
        tireMonitor.CheckTirePressure();
        Console.WriteLine();

        // 2. Light Control
        Console.WriteLine("\n=== LIGHT CONTROL ===");
        Console.WriteLine(lightControl.GetStatus());
        lightControl.SetLightMode("Full");
        Console.WriteLine(lightControl.GetStatus());
        Console.WriteLine("\nEnabling auto mode...");
        lightControl.EnableAutoMode();
        Console.WriteLine(lightControl.GetStatus());
        
        Console.WriteLine("\nSimulating environment changes...");
        for (int i = 0; i < 3; i++)
        {
            lightControl.UpdateAmbientLight();
            Console.WriteLine(lightControl.GetStatus());
        }
        Console.WriteLine();

        // 3. Reverse Alerts
        Console.WriteLine("\n=== REVERSE ALERTS ===");
        Console.WriteLine(reverseAlerts.GetStatus());
        Console.WriteLine("\nCar in reverse - checking for obstacles...");
        
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine($"\n--- Check {i + 1} ---");
            reverseAlerts.DetectObstacle();
            Console.WriteLine(reverseAlerts.GetStatus());
        }

        Console.WriteLine("\n==================================================");
        Console.WriteLine("          SIMULATION COMPLETE");
        Console.WriteLine("==================================================");

        adapter.Leave();
    }
}