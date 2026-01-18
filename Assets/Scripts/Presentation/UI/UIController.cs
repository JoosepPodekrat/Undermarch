using UnityEngine;
using Undermarch.Presentation.Sounds;

public class MenuManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject mainMenuPanel;
    public GameObject subMenus;

    [Header("Sub Menu Panels")]
    public GameObject optionsPanel;
    public GameObject levelPanel;
    public GameObject newGamePanel;
    [Header("Slideshow Panel")]
    public GameObject slideshowPanel;


    public void OpenOptions()
    {
        UIAudioManager.Instance?.PlayButtonClick();
        
        mainMenuPanel.SetActive(false);
        subMenus.SetActive(true);

        optionsPanel.SetActive(true);
        levelPanel.SetActive(false);
        if (newGamePanel != null) newGamePanel.SetActive(false);
    }

    public void OpenMainMenu()
    {
        UIAudioManager.Instance?.PlayButtonClick();
        
        subMenus.SetActive(false);
        slideshowPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }


    /// <summary>
    /// Opens the level selector panel. Called by New Game button.
    /// </summary>
    public void OpenLevelSelector()
    {
        UIAudioManager.Instance?.PlayButtonClick();
        
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
        UIAudioManager.Instance?.PlayButtonClick();
        
        Debug.Log("Quit button pressed");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void OpenSlideshow()
    {
        mainMenuPanel.SetActive(false); // hide main menu
        subMenus.SetActive(false);      // hide other panels
        slideshowPanel.SetActive(true); // show slideshow panel
    }
   


}
