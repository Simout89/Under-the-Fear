using UnityEngine;

public class WwiseEnterExitTriggerZone : MonoBehaviour
{
    [Header("Wwise Events")]
    public AK.Wwise.Event enterEvent;  // Drag-and-drop из Wwise Picker
    public AK.Wwise.Event exitEvent;   // Drag-and-drop из Wwise Picker

    [Header("Target")]
    public GameObject eventTarget;     // Объект, от которого проигрываются события

    void Start()
    {
        if (eventTarget == null)
            eventTarget = gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enterEvent != null)
            {
                enterEvent.Post(eventTarget);
                Debug.Log($"Triggered Wwise enter event '{enterEvent.Name}'");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (exitEvent != null)
            {
                exitEvent.Post(eventTarget);
                Debug.Log($"Triggered Wwise exit event '{exitEvent.Name}'");
            }
        }
    }
}
