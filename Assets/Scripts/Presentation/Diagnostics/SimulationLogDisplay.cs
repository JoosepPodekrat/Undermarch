using Undermarch.Simulation.Core;
using UnityEngine;

namespace Undermarch.Presentation.Diagnostics
{
    public class SimulationLogDisplay : MonoBehaviour
    {
        void Awake()
        {
            // Using DontDestroyOnLoad to ensure the logger persists across scene loads,
            // which is useful since the simulation runs in a separate scene.
            DontDestroyOnLoad(gameObject);
            SimulationLog.OnLog += HandleLog;
        }

        void OnDestroy()
        {
            SimulationLog.OnLog -= HandleLog;
        }

        private void HandleLog(string message)
        {
            Debug.Log($"[Simulation]: {message}");
        }
    }
}
