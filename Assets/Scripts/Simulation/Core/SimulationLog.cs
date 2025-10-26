namespace Undermarch.Simulation.Core
{
    public static class SimulationLog
    {
        public static event System.Action<string> OnLog;

        public static void Log(string message)
        {
            OnLog?.Invoke(message);
        }
    }
}
