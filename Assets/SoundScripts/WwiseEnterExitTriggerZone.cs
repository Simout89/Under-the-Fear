using UnityEngine;

public class WwiseEnterExitTriggerZone : MonoBehaviour
{
    [Header("Wwise Events")]
    public string enterEvent = "Play_Sound";  // Ивент при входе (нужно назначить)
    public string exitEvent = "Stop_Sound";   // Ивент при выходе (нужно назначить)

    [Header("Target")]
    public GameObject eventTarget;            // От чьего имени проигрываются события

    void Start()
    {
        if (eventTarget == null)
            eventTarget = gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Игрок должен быть с тегом!
        {
            if (!string.IsNullOrEmpty(enterEvent))
            {
                AkSoundEngine.PostEvent(enterEvent, eventTarget);
                Debug.Log($"Triggered Wwise enter event '{enterEvent}'");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(exitEvent))
            {
                AkSoundEngine.PostEvent(exitEvent, eventTarget);
                Debug.Log($"Triggered Wwise exit event '{exitEvent}'");
            }
        }
    }
}
