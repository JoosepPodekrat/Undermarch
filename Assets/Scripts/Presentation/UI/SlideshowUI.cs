using UnityEngine;
using UnityEngine.UI;

public class SlideshowUI : MonoBehaviour
{
    [Header("Slides")]
    public Sprite[] slides;               // Assign your slide sprites here
    private int currentSlideIndex = 0;

    [Header("UI References")]
    public Image displayImage;            // Assign the Panel's Image component
    public GameObject slideshowPanel;     // Usually the same GameObject this script is on

    private void OnEnable()
    {
        ShowSlide(currentSlideIndex);
    }

    public void ShowSlide(int index)
    {
        if (slides == null || slides.Length == 0)
        {
            Debug.LogWarning("SlideshowUI: No slides assigned!");
            return;
        }

        if (displayImage == null)
        {
            Debug.LogError("SlideshowUI: Display Image not assigned!");
            return;
        }

        currentSlideIndex = Mathf.Clamp(index, 0, slides.Length - 1);
        displayImage.sprite = slides[currentSlideIndex];
    }

    public void NextSlide()
    {
        if (slides == null || slides.Length == 0) return;

        currentSlideIndex++;
        if (currentSlideIndex >= slides.Length)
            currentSlideIndex = slides.Length - 1; // Stop at last slide
        ShowSlide(currentSlideIndex);
    }

    public void PreviousSlide()
    {
        if (slides == null || slides.Length == 0) return;

        currentSlideIndex--;
        if (currentSlideIndex < 0)
            currentSlideIndex = 0; // Stop at first slide
        ShowSlide(currentSlideIndex);
    }

    public void CloseSlideshow()
    {
        if (slideshowPanel != null)
            slideshowPanel.SetActive(false);
    }

    /// <summary>
    /// Call this from MenuManager to open the slideshow from the main menu
    /// </summary>
    public void OpenSlideshow()
    {
        if (slideshowPanel != null)
        {
            slideshowPanel.SetActive(true);
            currentSlideIndex = 0;
            ShowSlide(currentSlideIndex);
        }
        else
        {
            Debug.LogWarning("SlideshowUI: Slideshow panel not assigned!");
        }
    }

}
