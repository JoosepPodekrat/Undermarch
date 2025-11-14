namespace Undermarch.Simulation.Interfaces
{
    public enum TickMode
    {
        Paused,
        Step,
        Auto
    }

    /// <summary>
    /// Interface for the tick system that controls simulation execution.
    /// </summary>
    public interface ITickSystem
    {
        TickMode Mode { get; set; }
        int TicksPerSecond { get; set; }
        int CurrentTick { get; }

        void Tick();
        void Pause();
        void Resume();
        void Step();
    }
}
