using UnityEngine;

public class DictaphonePuzzle : MonoBehaviour, IClickable
{
    [SerializeField] private AK.Wwise.Event dictaphoneButtonEvent;

    public void Click()
    {
        dictaphoneButtonEvent.Post(gameObject);
    }
}
