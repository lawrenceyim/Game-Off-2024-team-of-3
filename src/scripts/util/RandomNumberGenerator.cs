using System;

public class RandomNumberGenerator {
    private static Random random = new Random();
    public static double GetRandomDoubleBetween(double min, double max) {
        return random.NextDouble() * (max - min) + min;
    }
}
