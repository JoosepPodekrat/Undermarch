namespace Undermarch.Simulation.Interfaces
{
    /// <summary>
    /// Provides deterministic random number generation for simulation reproducibility.
    /// </summary>
    public interface IRandomSource
    {
        int Next();
        int Next(int maxValue);
        int Next(int minValue, int maxValue);
        float NextFloat();
        float NextFloat(float minValue, float maxValue);
        bool NextBool();
    }
}
