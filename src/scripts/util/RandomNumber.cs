using System;

public class RandomNumber {
	private static readonly Random _random = new Random();
	public static double RandomDoubleBetween(double min, double max) {
		return _random.NextDouble() * (max - min) + min;
	}

	public static float RandomFloatBetween(float min, float max) {
		return (float)_random.NextDouble() * (max - min) + min;
	}

	// Start inclusive, end exclusive
	public static int RandomIntBetween(int min, int max) {
		return _random.Next(min, max);
	}
}
