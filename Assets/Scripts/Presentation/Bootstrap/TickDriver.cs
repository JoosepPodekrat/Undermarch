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

        void Awake()
        {
            _tickSystem = new TickSystem();
        }

        void Update()
        {
            if (paused) return;
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