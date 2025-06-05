using UnityEngine;
using AK.Wwise;

public class SceneStartMusicPlayer : MonoBehaviour
{
    public AK.Wwise.Event musicEvent;

    void Start()
    {
        if (AkSoundEngine.IsInitialized())
        {
            musicEvent.Post(gameObject);
        }
    }
}
