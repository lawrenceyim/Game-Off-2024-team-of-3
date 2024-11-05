using System;

public class RandomNumber {
    private static Random random = new Random();
    public static double RandomDoubleBetween(double min, double max) {
        return random.NextDouble() * (max - min) + min;
    }

    public static float RandomFloatBetween(float min, float max) {
        return (float)random.NextDouble() * (max - min) + min;
    }
}
