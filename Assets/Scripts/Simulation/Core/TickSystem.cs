namespace Undermarch.Simulation.Core
{
    public interface ITickSystem
    {
        int CurrentTick { get; }
        void Tick(); // advances simulation by 1 tick
    }

    public sealed class TickSystem : ITickSystem
    {
        public int CurrentTick { get; private set; }
        public event System.Action<int> OnTick;

        public void Tick()
        {
            CurrentTick++;
            OnTick?.Invoke(CurrentTick);
        }
    }
}