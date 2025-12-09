using System;
using EVSystem.Components;
using EVSystem.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EVSystem.Member2Tests
{
	[TestClass]
	public class ClimateControlTests
	{
		private IClimateControl CreateClimate(float initialTemp = 20.0f)
		{
			return new ClimateControl(initialTemp);
		}

		[TestMethod]
		public void ClimateControl_InitialValues_AreCorrect()
		{
			var climate = CreateClimate(21.0f);

			Assert.AreEqual(21.0f, climate.CurrentTemperature, 0.01, "Current temp should match initial.");
			Assert.AreEqual(21.0f, climate.TargetTemperature, 0.01, "Target temp should start equal to current.");
			Assert.IsFalse(climate.PreConditioningEnabled, "Pre-conditioning should be OFF by default.");
		}

		[TestMethod]
		public void SetTemperature_ClampsToAllowedRange()
		{
			var climate = CreateClimate(20.0f);

			climate.SetTemperature(999);   // way too high
			Assert.IsTrue(climate.TargetTemperature <= 60.0f, "Target should be clamped to max.");

			climate.SetTemperature(-999);  // way too low
			Assert.IsTrue(climate.TargetTemperature >= -30.0f, "Target should be clamped to min.");
		}

		[TestMethod]
		public void UpdateClimate_MovesTemperatureTowardTarget()
		{
			var climate = CreateClimate(20.0f);

			climate.SetTemperature(22.0f);  // 2°C above

			climate.UpdateClimate(); // +0.5
			Assert.AreEqual(20.5f, climate.CurrentTemperature, 0.2, "Temp should move closer to target.");

			// Call a few more times to reach ~22
			climate.UpdateClimate();
			climate.UpdateClimate();
			climate.UpdateClimate();

			Assert.AreEqual(22.0f, climate.CurrentTemperature, 0.2, "Temp should be close to target after multiple steps.");
		}

		[TestMethod]
		public void PreConditioning_TogglesCorrectly()
		{
			var climate = CreateClimate();

			Assert.IsFalse(climate.PreConditioningEnabled, "Should start OFF.");

			climate.EnablePreConditioning();
			Assert.IsTrue(climate.PreConditioningEnabled, "Should turn ON.");

			climate.DisablePreConditioning();
			Assert.IsFalse(climate.PreConditioningEnabled, "Should turn OFF again.");
		}

		[TestMethod]
		public void GetStatus_ReturnsReadableText()
		{
			var climate = CreateClimate(19.5f);
			climate.SetTemperature(23.0f);
			climate.EnablePreConditioning();

			var status = climate.GetStatus();

			StringAssert.Contains(status, "Climate", "Status string should mention 'Climate'.");
			StringAssert.Contains(status, "Current", "Status string should mention 'Current'.");
			StringAssert.Contains(status, "Target", "Status string should mention 'Target'.");
		}
	}
}
