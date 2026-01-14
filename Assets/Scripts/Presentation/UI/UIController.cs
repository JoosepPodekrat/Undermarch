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
        newGamePanel.SetActive(false);
    }

    public void OpenMainMenu()
    {
        subMenus.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenLevelLoader()
    {
        mainMenuPanel.SetActive(false);
        subMenus.SetActive(true);

        optionsPanel.SetActive(false);
        levelPanel.SetActive(true);
        newGamePanel.SetActive(false);
    }

    public void OpenNewGame()
    {
        mainMenuPanel.SetActive(false);
        subMenus.SetActive(true);

        optionsPanel.SetActive(false);
        levelPanel.SetActive(false);
        newGamePanel.SetActive(true);
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
