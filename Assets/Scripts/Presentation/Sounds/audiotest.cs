using UnityEngine;
using Undermarch.Presentation;

public class AudioTest : MonoBehaviour
{
    public Sound[] sounds; // Assign these in the inspector

    private void Start()
    {
        if (sounds == null || sounds.Length == 0)
        {
            Debug.LogWarning("No sounds assigned!");
            return;
        }

        // Pick the first sound in the array
        Sound s = sounds[0];

        if (s.clip == null)
        {
            Debug.LogWarning($"Sound '{s.name}' has no AudioClip assigned!");
            return;
        }

        // Add an AudioSource component dynamically
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = s.clip;
        source.volume = 0.5f;
        source.pitch = s.pitch;
        source.spatialBlend = 0f; // 2D sound
        source.playOnAwake = false;

        Debug.Log($"Playing sound: {s.name}");
        source.Play();
    }
}
