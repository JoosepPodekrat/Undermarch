using Undermarch.Simulation.Core;
using UnityEngine;

namespace Undermarch.Presentation.Bootstrap
{
    public class TickDriver : MonoBehaviour
    {
        [Range(1,60)] public int ticksPerSecond = 10;
        public bool paused;
        private float _accum;
        private ITickSystem _tickSystem;

        void Start()
        {
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.InitializeTickDriver(this);
            }
            else
            {
                Debug.LogError("TickDriver: GameManager.Instance is null in Start. Cannot initialize.");
            }
        }

        public void SetTickSystem(ITickSystem tickSystem)
        {
            _tickSystem = tickSystem;
        }

        void Update()
        {
            if (paused || _tickSystem == null) return;

            // Safety check to prevent division by zero
            if (ticksPerSecond <= 0)
            {
                ticksPerSecond = 1;
            }

            _accum += Time.deltaTime;
            float interval = 1f / ticksPerSecond;
            while (_accum >= interval)
            {
                _accum -= interval;
                _tickSystem.Tick();
            }
        }

        public void StepOneTick()
        {
            _tickSystem.Tick();
        }
    }
}