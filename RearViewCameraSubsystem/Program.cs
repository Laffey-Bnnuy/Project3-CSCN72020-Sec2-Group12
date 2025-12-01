using System;
using System.Threading;
using EV_SCADA.Modules;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== REAR VIEW CAMERA SUBSYSTEM — SPRINT 2 DEMO ===\n");

        var camera = new RearViewCamera();

        // Subscribe to camera events
        camera.OnFrameUpdated += frame =>
        {
            Console.WriteLine($"[FRAME] {frame}");
        };

        camera.OnAlert += alert =>
        {
            Console.WriteLine($"[ALERT] {alert}");
        };

        //  Activate the camera
        Console.WriteLine("Activating camera...");
        camera.Activate();
        Thread.Sleep(1500); // record a few frames

        //  Take a snapshot
        Console.WriteLine("\nTaking snapshot...");
        var snapshot = camera.CaptureSnapshot();
        Console.WriteLine($"Snapshot: {snapshot}");


        //  Show how many frames recorded so far
        Console.WriteLine($"\nRecorded Frames Count: {camera.RecordedFrames.Count}");

        //  Playback demo
        Console.WriteLine("\nPlayback recorded frames:");
        foreach (var f in camera.Playback())
            Console.WriteLine($" - {f}");

        //  Sensitivity demonstration
        Console.WriteLine("\nTesting high sensitivity alert...");
        camera.AlertSensitivity = Sensitivity.High;

        // Manually simulate distance alert
        var handleDistance = typeof(RearViewCamera)
            .GetMethod("HandleDistance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        handleDistance?.Invoke(camera, new object[] { 25 });

        //  Mute alerts
        Console.WriteLine("\nMuting alerts...");
        camera.AlertsMuted = true;
        handleDistance?.Invoke(camera, new object[] { 15 });

        //  Trigger camera fault
        Console.WriteLine("\nTriggering fault...");
        camera.TriggerFault();

        // Fault should stop alerts:
        handleDistance?.Invoke(camera, new object[] { 10 });

        //  Deactivate camera
        Console.WriteLine("\nDeactivating camera...");
        camera.Deactivate();

        Console.WriteLine("\n=== END OF DEMO ===");
    }
}

