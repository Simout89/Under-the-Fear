using UnityEngine;

public class RTPCZoneReset : MonoBehaviour
{
    public MusicRTPCController musicController; // <-- установить в инспекторе

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && musicController != null)
        {
            musicController.SetIntensity(0f); // Сброс интенсивности
            AkSoundEngine.SetRTPCValue(musicController.rtpcName, 0f, musicController.gameObject);
            Debug.Log($"RTPC '{musicController.rtpcName}' set to 0 due to zone trigger");
        }
    }
}
