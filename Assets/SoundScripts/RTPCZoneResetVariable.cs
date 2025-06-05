using UnityEngine;

public class RTPCZoneResetVariable : MonoBehaviour
{
    [Header("Music controller (установить в инспекторе)")]
    public MusicRTPCController musicController;

    [Header("New RTPC Value")]
    [Range(0f, 100f)]
    public float resetValue = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && musicController != null)
        {
            musicController.SetIntensity(resetValue);
            AkSoundEngine.SetRTPCValue(musicController.rtpcName, resetValue, musicController.gameObject);
            Debug.Log($"RTPC '{musicController.rtpcName}' set to {resetValue} due to zone trigger");
        }
    }
}
