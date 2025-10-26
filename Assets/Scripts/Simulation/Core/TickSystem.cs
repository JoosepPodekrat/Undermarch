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

        private bool _isRunning = true;

        public void Tick()
        {
            if (!_isRunning) return;

            currentTick++;
            OnTick?.Invoke(currentTick);
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}