namespace Undermarch.Simulation.Core
{
    public interface ITickSystem
    {
        int currentTick { get; }
        void Tick(); // advances simulation by 1 tick
    }

    public sealed class TickSystem : ITickSystem
    {
        public int currentTick { get; private set; }
        public event System.Action<int> OnTick;

        public void Tick()
        {
            currentTick++;
            OnTick?.Invoke(currentTick);
        }
    }
}