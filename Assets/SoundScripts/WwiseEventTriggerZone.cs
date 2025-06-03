using UnityEngine;

public class WwiseEventTriggerZone : MonoBehaviour
{
    public string eventName = "Play_Sound"; // Название события из Wwise
    public GameObject eventTarget;          // Объект, от имени которого звучит событие

    private bool hasActivated = false;

    void Start()
    {
        if (eventTarget == null)
            eventTarget = gameObject; // Если не указано — использовать сам объект
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player"))
        {
            AkSoundEngine.PostEvent(eventName, eventTarget);
            Debug.Log($"Wwise event '{eventName}' triggered from {eventTarget.name}");
            hasActivated = true;
        }
    }
}
