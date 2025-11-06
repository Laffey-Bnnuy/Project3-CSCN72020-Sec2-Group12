using System;
using EVSystem.Interfaces;
using EVSystem.Components;
using EVSystem.Mock;

class Program
{
    static void Main(string[] args)
    {
        var adapter = new MockJ1939Adapter();

        IBattery battery = new BatteryMonitor(adapter);
        IChargingControl charger = new ChargingControl(adapter);

        Console.WriteLine("EV System Simulation");

        Console.WriteLine("\nInitial battery status:");
        Console.WriteLine(battery.GetStatus());

        Console.WriteLine("\nSet battery mode to sport");
        battery.SetBatteryMode("Sport");
        Console.WriteLine("Battery current status: \n");
        Console.WriteLine(battery.GetStatus());

        Console.WriteLine("\nBattery now only have 80%");
        battery.UpdateBatteryLevel(80);

        Console.WriteLine("Battery current status: \n");
        Console.WriteLine(battery.GetStatus());



        Console.WriteLine("\nSchedule charging for tonight");
        charger.ScheduleCharging("23:00 - 05:00");

        Console.WriteLine("\nThe schedule is: "+charger.GetSchedule());

        adapter.Leave();
    }
}
