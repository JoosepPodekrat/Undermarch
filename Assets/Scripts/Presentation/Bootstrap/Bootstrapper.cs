using UnityEngine;
using UnityEngine.SceneManagement;

namespace Undermarch.Presentation.Bootstrap
{
    public class Bootstrapper : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log("Bootstrapper: Awake() called.");
            bool simulationSceneLoaded = IsSceneLoaded("Simulation");
            Debug.Log($"Bootstrapper: IsSceneLoaded('Simulation') returned {simulationSceneLoaded}.");

            if (simulationSceneLoaded)
            {
                Debug.Log("Bootstrapper: Core scenes are already loaded. Skipping additive load.");
                return;
            }

            Debug.Log("Bootstrapper: Loading core scenes...");
            LoadCoreScenes();
        }

        private void LoadCoreScenes()
        {
            SceneManager.LoadScene("Simulation", LoadSceneMode.Additive);
            SceneManager.LoadScene("Rendering", LoadSceneMode.Additive);
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private bool IsSceneLoaded(string sceneName)
        {
            Debug.Log($"IsSceneLoaded: Checking for scene '{sceneName}'. Total scenes loaded: {SceneManager.sceneCount}");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log($"IsSceneLoaded: Found scene '{scene.name}' at index {i}.");
                if (scene.name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}