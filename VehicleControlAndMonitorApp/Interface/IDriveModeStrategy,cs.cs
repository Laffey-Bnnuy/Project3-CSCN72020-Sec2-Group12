namespace EVSystem.Interfaces
{
	/// <summary>
	/// Strategy contract for a single drive mode (Eco / Normal / Sport).
	/// </summary>
	public interface IDriveModeStrategy
	{
		string Name { get; }

		/// <summary>
		/// Conceptual power factor for the mode (0..1).
		/// Used to reason about performance vs efficiency.
		/// </summary>
		float PowerFactor { get; }

		/// <summary>
		/// Regeneration strength (0..1), where 1 = strongest regen.
		/// </summary>
		float RegenStrength { get; }
	}
}
