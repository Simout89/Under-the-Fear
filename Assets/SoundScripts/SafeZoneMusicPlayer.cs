using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    [Header("Wwise Event")]
    public AK.Wwise.Event onEnterEvent;

    [Header("RTPC Controller (на объекте игрока)")]
    public MusicRTPCController rtpcController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (onEnterEvent != null)
            {
                onEnterEvent.Post(other.gameObject);
                Debug.Log($"Event '{onEnterEvent.Name}' posted on enter.");
            }
            else
            {
                Debug.LogWarning("No Wwise event assigned for entering the zone.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rtpcController != null)
            {
                rtpcController.SetIntensity(0f);
                Debug.Log("Exited zone, RTPC intensity reset.");
            }
            else
            {
                Debug.LogWarning("MusicRTPCController reference not set.");
            }
        }
    }
}
