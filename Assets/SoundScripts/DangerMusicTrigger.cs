using UnityEngine;

public class DangerMusicTrigger : MonoBehaviour
{
    [Header("Wwise Settings")]
    public string switchGroup = "Danger_Music";
    public string switchState = "Main_Loop";
    public AK.Wwise.Event musicEvent;
    public GameObject musicEmitter;
    public bool postEvent = false;

    private bool triggered = false;

    public void ResetTrigger()
    {
        triggered = false;
    }

    // Новый публичный метод — вызывается вручную
    public void TriggerMusic()
    {
        if (triggered)
            return;

        triggered = true;

        if (musicEmitter != null)
        {
            AkSoundEngine.SetSwitch(switchGroup, switchState, musicEmitter);

            if (postEvent && musicEvent != null)
            {
                musicEvent.Post(musicEmitter);
            }
        }
        else
        {
            Debug.LogWarning("MusicEmitter not assigned on " + gameObject.name);
        }
    }
}
