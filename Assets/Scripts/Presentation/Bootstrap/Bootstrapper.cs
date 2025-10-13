using UnityEngine;
using UnityEngine.SceneManagement;

namespace Undermarch.Presentation.Bootstrap
{
    public class Bootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var active = SceneManager.GetActiveScene();
            if (active.name != "Bootstrap") return;
            SceneManager.LoadScene("Simulation", LoadSceneMode.Additive);
            SceneManager.LoadScene("Rendering", LoadSceneMode.Additive);
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }
    }
}