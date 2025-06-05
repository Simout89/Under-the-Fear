using UnityEngine;

public class MusicRTPCController : MonoBehaviour
{
    public string rtpcName = "MusicIntensity";
    public float intensity = 0f;
    public float maxIntensity = 100f;

    // Выбор события через Wwise Picker
    public AK.Wwise.Event musicEvent;

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

    public void PlayMusic()
    {
        if (musicEvent != null)
        {
            musicEvent.Post(gameObject);
            Debug.Log($"Event '{musicEvent.Name}' posted.");
        }
        else
        {
            Debug.LogWarning("No music event assigned.");
        }
    }
}
