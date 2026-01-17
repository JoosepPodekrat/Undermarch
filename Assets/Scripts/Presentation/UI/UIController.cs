using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject mainMenuPanel;
    public GameObject subMenus;

    [Header("Sub Menu Panels")]
    public GameObject optionsPanel;
    public GameObject levelPanel;
    public GameObject newGamePanel;

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        subMenus.SetActive(true);

        optionsPanel.SetActive(true);
        levelPanel.SetActive(false);
        if (newGamePanel != null) newGamePanel.SetActive(false);
    }

    public void OpenMainMenu()
    {
        subMenus.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Opens the level selector panel. Called by New Game button.
    /// </summary>
    public void OpenLevelSelector()
    {
        mainMenuPanel.SetActive(false);
        subMenus.SetActive(true);

        optionsPanel.SetActive(false);
        levelPanel.SetActive(true);
        if (newGamePanel != null) newGamePanel.SetActive(false);
        
        // Refresh the level buttons to show current unlock status
        var levelSelectorUI = levelPanel.GetComponent<Undermarch.Presentation.UI.LevelSelectorUI>();
        if (levelSelectorUI != null)
        {
            levelSelectorUI.RefreshButtons();
        }
    }

    public void OpenNewGame()
    {
        // Redirect to level selector
        OpenLevelSelector();
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit button pressed");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
