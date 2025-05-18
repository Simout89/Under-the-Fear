using UnityEngine;

public class MusicRTPCController : MonoBehaviour
{
    [Header("Wwise Settings")]
    public string musicEventName = "Play_Music"; // Имя ивента вводится через инспектор
    public string rtpcName = "MusicIntensity";

    [Header("Music Intensity")]
    public float intensity = 0f;
    public float maxIntensity = 100f;

    public void SetIntensity(float newIntensity)
    {
        intensity = Mathf.Clamp(newIntensity, 0f, maxIntensity);
        AkSoundEngine.SetRTPCValue(rtpcName, intensity, gameObject);
        Debug.Log("Intensity set to: " + intensity);
    }

    public void IncreaseIntensity(float step)
    {
        intensity = Mathf.Clamp(intensity + step, 0f, maxIntensity);
        AkSoundEngine.SetRTPCValue(rtpcName, intensity, gameObject);
        Debug.Log($"RTPC '{rtpcName}' set to {intensity}");
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(musicEventName))
        {
            AkSoundEngine.PostEvent(musicEventName, gameObject);
        }
        else
        {
            Debug.LogWarning("Music event name is not set in the inspector.");
        }
    }
}
