using EVSystem.Components;
using EVSystem.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EVSystem.Member2Tests
{
	// Simple fake battery for testing DriveModeSelector only
	public class FakeBatteryForDriveMode : IBattery
	{
		public float BatteryLevel { get; private set; }
		public float Temperature { get; set; }
		public float RemainingDriveTime { get; private set; }
		public float RemainingKm { get; private set; }
		public string BatteryMode { get; private set; } = "Normal";
		public bool AutoMode { get; private set; }

		public void LoadNextData()
		{
			// Not needed for these tests
		}

		public void UpdateBatteryLevel(float newVal)
		{
			BatteryLevel = newVal;
		}

		public void SetBatteryMode(string mode)
		{
			BatteryMode = mode;
		}

		public void EnableAutoMode() => AutoMode = true;
		public void DisableAutoMode() => AutoMode = false;

		public string GetStatus() => $"Battery {BatteryLevel}% Mode={BatteryMode}";
	}

	[TestClass]
	public class DriveModeSelectorTests
	{
		private (DriveModeSelector selector, FakeBatteryForDriveMode battery) CreateSelector()
		{
			var battery = new FakeBatteryForDriveMode();
			var selector = new DriveModeSelector(battery);
			return (selector, battery);
		}

		[TestMethod]
		public void DefaultMode_IsNormal_AndAppliedToBattery()
		{
			var (selector, battery) = CreateSelector();

			Assert.AreEqual("Normal", selector.CurrentMode, "Default mode should be Normal.");
			Assert.AreEqual("Normal", battery.BatteryMode, "Battery mode should match current drive mode.");
		}

		[TestMethod]
		public void TrySetMode_ValidModes_UpdateCurrentMode_AndBattery()
		{
			var (selector, battery) = CreateSelector();

			var ecoChanged = selector.TrySetMode("Eco");
			Assert.IsTrue(ecoChanged, "Setting Eco should succeed.");
			Assert.AreEqual("Eco", selector.CurrentMode);
			Assert.AreEqual("Eco", battery.BatteryMode);

			var sportChanged = selector.TrySetMode("Sport");
			Assert.IsTrue(sportChanged, "Setting Sport should succeed.");
			Assert.AreEqual("Sport", selector.CurrentMode);
			Assert.AreEqual("Sport", battery.BatteryMode);
		}

		[TestMethod]
		public void TrySetMode_Invalid_DoesNotChangeMode()
		{
			var (selector, battery) = CreateSelector();

			var initialMode = selector.CurrentMode;
			var initialBatteryMode = battery.BatteryMode;

			var result = selector.TrySetMode("Turbo"); // not a real mode
			Assert.IsFalse(result, "Invalid mode name should return false.");
			Assert.AreEqual(initialMode, selector.CurrentMode, "Mode should not change.");
			Assert.AreEqual(initialBatteryMode, battery.BatteryMode, "Battery should keep previous mode.");
		}

		[TestMethod]
		public void GetStatus_ReturnsDriveModeSummary()
		{
			var (selector, _) = CreateSelector();

			selector.TrySetMode("Eco");
			var status = selector.GetStatus();

			StringAssert.Contains(status, "Drive", "Status should mention drive mode.");
			StringAssert.Contains(status, "Eco", "Status should mention current mode name.");
		}
	}
}
